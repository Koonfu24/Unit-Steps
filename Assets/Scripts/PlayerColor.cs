using UnityEngine;
using Unity.Netcode;

public class PlayerColor : NetworkBehaviour
{
    public SpriteRenderer sprite; 

    private NetworkVariable<Color> playerColor =
        new NetworkVariable<Color>(
            Color.white,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private void Awake()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        // ทุก client จะโดนเรียก
        ApplyColor(playerColor.Value);

        playerColor.OnValueChanged += (oldColor, newColor) =>
        {
            ApplyColor(newColor);
        };

        // ถ้าเป็น server ให้สุ่มสี
        if (IsServer)
        {
            AssignUniqueColor();
        }
    }

    void ApplyColor(Color c)
    {
        sprite.color = c;
    }

    void AssignUniqueColor()
    {
        Color uniqueColor = ColorManager.Instance.GetUniqueColor();
        playerColor.Value = uniqueColor;
    }
}
