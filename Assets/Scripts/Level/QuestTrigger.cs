using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public enum Type {NextStage, StartQuest}
    public Type TriggetType;
    public int QuestIndex;
    public int Stage;
    public QuestSystem QuestSystem;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (TriggetType)
        {
            case Type.NextStage:
                if (collision.transform.root.tag == "Player")
                {
                    QuestSystem.TriggerEnter(QuestIndex, Stage);
                }
                break;
            case Type.StartQuest:
                if (collision.transform.root.tag == "Player")
                {
                    QuestSystem.QuestStart(QuestIndex);
                }
                break;
        }
        
        
    }
}
