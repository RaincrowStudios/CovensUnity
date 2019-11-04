using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FAQManager;

public class FAQData : EnhancedScrollerCellView
{

    public TextMeshProUGUI question;
    public TextMeshProUGUI answer;

    public Button btn;
    private void Awake()
    {
        btn.onClick.AddListener(() =>
  {
      answer.gameObject.SetActive(!answer.gameObject.activeInHierarchy);
  });
    }

    public void SetData(QAdata data)
    {
        // update the UI text with the cell data

        question.text = data.Question;
        answer.text = data.Answer;
    }
}
