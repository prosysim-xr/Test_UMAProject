using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.PoseTools;
using UMA.CharacterSystem;

public class UMAExpressions : MonoBehaviour
{
    public UMAExpressionPlayer expressionPlayer;
    public bool ready = false;

    //use this for initialization
    private void Start()
    {
        UMADynamicAvatar avatar = GetComponent<UMADynamicAvatar>();

        if (avatar.umaData == null)
        {
            avatar.Initialize();
        }
        avatar.umaData.OnCharacterCreated += AddExpressions;
    }

    public void AddExpressions(UMAData umaData)
    {
        UMAExpressionSet expressionSet = umaData.umaRecipe.raceData.expressionSet;
        expressionPlayer = umaData.gameObject.AddComponent<UMAExpressionPlayer>();
        expressionPlayer.expressionSet = expressionSet;
        expressionPlayer.umaData = umaData;
        expressionPlayer.Initialize();

        ready = true;
    }
}
