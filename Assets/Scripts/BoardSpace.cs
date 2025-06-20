using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Vector2Int position;

    [SerializeField]
    private Image image;

    private Shape currentShape = Shape.EMPTY;

    private void OnEnable()
    {
        NetworkUtility.C_BoardMove += OnBoardMove;
    }

    private void OnDisable()
    {
        NetworkUtility.C_BoardMove -= OnBoardMove;
    }

    private void OnBoardMove(NetworkMessage message)
    {
        BoardMoveMessage msg = message as BoardMoveMessage;
        if (msg.movePos == position)
        {
            Sprite newSprite = msg.shape == Shape.CROSS ? TicTacToe.instance.crossSprite : TicTacToe.instance.circleSprite;
            image.sprite = newSprite;
        }
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentShape == Shape.EMPTY)
        {
            Shape assignedShape = Player.instance.assignedShape;

            BoardMoveMessage moveToSend = new BoardMoveMessage();
            moveToSend.movePos = position;
            moveToSend.shape = assignedShape;
            ClientBehaviour.instance.SendToServer(moveToSend);
        }
    }
}
