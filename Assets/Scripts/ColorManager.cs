using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    public List<Color> availableColors = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    private HashSet<Color> usedColors = new HashSet<Color>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Color GetUniqueColor()
    {
        foreach (Color c in availableColors)
        {
            if (!usedColors.Contains(c))
            {
                usedColors.Add(c);
                return c;
            }
        }

        
        return Random.ColorHSV();
    }
}
