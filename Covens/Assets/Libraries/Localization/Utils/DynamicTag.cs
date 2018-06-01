// Oktagon Games
// FileName: DynamicTag.cs
// Author: Oktagon
// Created on: 2016/10/13
//
using System;

namespace Oktagon.Localization {

	// a class that represent 1 dynamic-replaced tag the moment we call the .GetText(x, true).
	[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
	public class DynamicTag {
		protected const string PARAMETER_TAG = "=N";

		// identifier to allow remove, get, change. etc. e.g.: "PlayerName". Can have repeated in the tag list
		public string m_sName;

		// what to replace in text. e.g.: "<dPlayerName>", "<dLokaki=N>
		protected string m_sReplacement;

		// function to replace the tag. e.g.: {return Player.name;}
		protected System.Func<string> m_pFuntion;
		protected System.Func<string, string> m_pFuntionParam;

		// this is true if tag replacement Contains PARAMETER_TAG. will pass N as parameter in delegate
		protected bool m_bHasParameter; 	
		protected string m_sParameterPre; 	// "<dLokaki="
		protected string m_sParameterPost; 	// ">"


		public DynamicTag(string sName, string sReplacement) {
			this.m_sName = sName;
			this.m_sReplacement = sReplacement;
		}

		public DynamicTag(string sName, string sReplacement, System.Func<string> pFuntion) 
			: this (sName, sReplacement)
		{
			this.m_pFuntion = pFuntion;
		}

		public DynamicTag(string sName, string sReplacement, System.Func<string, string> pFuntion)
			: this (sName, sReplacement)
		{
			m_pFuntionParam = pFuntion;
			int iIndex = sReplacement.IndexOf(PARAMETER_TAG);
			if (iIndex >= 0) {
				m_bHasParameter = true;
				m_sParameterPre = sReplacement.Substring(0, iIndex);
				m_sParameterPost = sReplacement.Substring(iIndex + PARAMETER_TAG.Length);
			}

		}

		/// <summary>
		/// replaces the source and returns the new string.
		/// </summary>
		public string Replace(string sSource) {
			string sNewText;

			if (m_bHasParameter) {
				if (m_pFuntionParam == null) {
					sNewText = sSource;
				}
				else {

					// a more complex replacement with parameters
					var msg = new System.Text.StringBuilder(string.Empty);

					int iCurrentIndex = 0;
					while (true) {
						int iIndex = sSource.IndexOf(m_sParameterPre, iCurrentIndex);

						// found index?
						if (iIndex != -1) {

							int iIndexEnd = sSource.IndexOf(m_sParameterPost, m_sParameterPre.Length + iIndex);

							if (iIndexEnd != -1) {
								int iStart = iIndex + m_sParameterPre.Length +1; // +1 is for '=' char
								string sParameter = sSource.Substring(
									iStart, 
									iIndexEnd - iStart
								);

								// get new value with parameter.
								string sValue = m_pFuntionParam(sParameter);

								// append values, and continue.
								msg.Append(sSource.Substring(iCurrentIndex, iIndex - iCurrentIndex));
								msg.Append(sValue);

								iCurrentIndex = iIndexEnd + m_sParameterPost.Length;

							}
							else {
								msg.Append(sSource.Substring(iCurrentIndex));
								break;
							}

						}
						else {
							msg.Append(sSource.Substring(iCurrentIndex));
							break;
						}

					}

					sNewText = msg.ToString();
				}
			}
			else {
				if (m_pFuntion == null) {
					sNewText = sSource;
				}
				else {
					// simple replacement
					string sValue = m_pFuntion();
					sNewText = sSource.Replace(m_sReplacement, sValue);
				}
			}

			return sNewText;
		}



		/// <summary>
		/// Returns true if the given sSource has the excpected tag
		/// </summary>
		public bool Matches(string sSource) {
			if (m_bHasParameter) {
				return sSource.Contains(m_sParameterPre);
			}
			else {
				return sSource.Contains(m_sReplacement);
			}
		}

		public string DebuggerDisplay {
			get {
				string sValue = string.Empty;
				if (m_pFuntion != null) {
					sValue = m_pFuntion();
				}

				return string.Format(
					"\"{0}\", replaces \"{1}\" to \"{2}\"",
					m_sName,
					m_sReplacement,
					sValue
				);
			}
		}
	}


}

