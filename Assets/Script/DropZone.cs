using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public GameObject[] Cards;
    public GameObject CardModel;
    public void AddCard(int index)
    { CardController c; 
        foreach(GameObject obj in Cards)
        {

           c = obj.GetComponent<CardController>();
            Debug.Log(c.getID());
            if(c.getID() == index)
            {
                GameObject newCard = GameObject.Instantiate(CardModel);
                newCard.AddComponent<CardController>();
                CardController nC = newCard.GetComponent<CardController>();
                nC.cardName = c.cardName;
                nC.image = c.image;
                newCard.AddComponent<EventTrigger>();
                EventTrigger et = newCard.GetComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                BoardManager bManager = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<BoardManager>();
				GameObject dUI = GameObject.FindGameObjectWithTag ("DetailUI");
				WakeDetailUI detailUI = dUI.GetComponent<WakeDetailUI>();
				entry.callback.AddListener((eventData) => {detailUI.WakeUI(newCard);});//bManager.Summon(newCard); });
                et.triggers.Add(entry);

                newCard.transform.parent = this.transform;
                foreach(Transform child in newCard.transform)
                {
                    if(child.name.ToLower() == "image")
                    {
                        Image im = child.gameObject.GetComponent<Image>();
                        im.sprite = c.getSprite();
                    }else if(child.name.ToLower() == "card title")
                    {
                        Text t = child.gameObject.GetComponent<Text>();
                        t.text = c.cardName;
                    }else if(child.name.ToLower() == "card description")
                    {
                        
                        Text t = child.gameObject.GetComponent<Text>();
                        t.text = c.getDescription();
                    }
                }
                newCard.transform.localScale = new Vector3(1, 1, 1);       
                break;
            }
        }
    }
	public void OnPointerEnter(PointerEventData eventData) {
		//Debug.Log("OnPointerEnter");
		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.placeholderParent = this.transform;
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		//Debug.Log("OnPointerExit");
		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo;
		}
	}
	
	public void OnDrop(PointerEventData eventData) {
		Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.parentToReturnTo = this.transform;
		}
		if(gameObject.name == "Tabletop")
		Destroy(GameObject.Find(eventData.pointerDrag.name),.5f);
	}
}
