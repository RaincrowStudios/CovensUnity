using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Raincrow.FTF
{
    [CustomEditor(typeof(FTFHighlight))]
    public class FTFHighlightInspector : FTFRectBaseInspector
    {
        public override string FieldName => "highlight";
    }
}