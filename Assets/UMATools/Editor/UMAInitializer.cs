using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UMA;
using UMA.CharacterSystem;

public class UMAInitializer
{
    [MenuItem("UMATools/Init UMA")]
    static void InitializeUMA()
    {
        //UMA
        GameObject uma = new GameObject();
        uma.name = "UMA";
        GameObject parent;
        if (parent = Selection.activeGameObject)
        {
            uma.transform.SetParent(parent.transform);
        }

        //Races
        GameObject races = new GameObject();
        races.name = "RaceLibrary";
        races.transform.SetParent(uma.transform);
        RaceLibrary raceLibrary = races.AddComponent<RaceLibrary>();

        string[] racesData = AssetDatabase.FindAssets("t: UMA.RaceData", null);
        foreach (string data in racesData)
        {
            string racePath = AssetDatabase.GUIDToAssetPath(data);
            UMA.RaceData raceAsset = AssetDatabase.LoadAssetAtPath<UMA.RaceData>(racePath);
            raceLibrary.AddRace(raceAsset);
        }

        //Slots
        GameObject slots = new GameObject();
        slots.name = "SlotLibrary";
        slots.transform.SetParent(uma.transform);

        SlotLibrary slotLibrary = slots.AddComponent<SlotLibrary>();

        string[] slotsData = AssetDatabase.FindAssets("t: UMA.SlotDataAsset", null);
        foreach (string data in slotsData)
        {
            string slotPath = AssetDatabase.GUIDToAssetPath(data);
            UMA.SlotDataAsset slotAsset = AssetDatabase.LoadAssetAtPath<UMA.SlotDataAsset>(slotPath);
            slotLibrary.AddSlotAsset(slotAsset);
        }

        //OverLays (-> overlays are synonym for texture)
        GameObject overlays = new GameObject();
        overlays.name = "OverlayLibrary";
        overlays.transform.SetParent(uma.transform);
        OverlayLibrary overlayLibrary = overlays.AddComponent<OverlayLibrary>();

        string[] overlaysData = AssetDatabase.FindAssets("t: UMA.OverlayDataAsset", null);
        foreach (string data in overlaysData)
        {
            string overlayPath = AssetDatabase.GUIDToAssetPath(data);
            UMA.OverlayDataAsset overlayAsset = AssetDatabase.LoadAssetAtPath<UMA.OverlayDataAsset>(overlayPath);
            overlayLibrary.AddOverlayAsset(overlayAsset);
        }


        //Context
        GameObject contextObj = new GameObject();
        contextObj.name = "context";
        contextObj.transform.SetParent(uma.transform);
        UMAContext context = contextObj.AddComponent<UMAContext>();
        context.raceLibrary = raceLibrary;
        context.slotLibrary = slotLibrary;
        context.overlayLibrary = overlayLibrary;

        //Generator (->similar to context way can be done but here instantiation  is done because it is easier according to Tobais Phillip)
        UMAGenerator generator = MonoBehaviour.Instantiate(AssetDatabase.LoadAssetAtPath<UMAGenerator>("Assets/UMATools/Prefabs/UMAGenerator.prefab"));
        generator.name = "UMAGenerator";
        generator.transform.SetParent(uma.transform);
        UMA.UMADefaultMeshCombiner meshCombiner = generator.gameObject.AddComponent<UMA.UMADefaultMeshCombiner>();
        generator.meshCombiner = meshCombiner;

    }

    [MenuItem("UMATools/TestUMA ")]
    static void TestUMA()
    {
        GameObject testUMA = new GameObject();
        testUMA.name = "Test UMA";
        UMADynamicAvatar avatar = testUMA.AddComponent<UMADynamicAvatar>();
        avatar.loadOnStart = true;

        //Recipe
        string[] hugos = AssetDatabase.FindAssets("Harold t:UMATextRecipe");
        string hugoPath = AssetDatabase.GUIDToAssetPath(hugos[0]);
        avatar.umaRecipe = AssetDatabase.LoadAssetAtPath<UMATextRecipe>(hugoPath);

        //Animator
        string[] animators = AssetDatabase.FindAssets("Locomotion t:RuntimeAnimatorController");
        string animatorPath = AssetDatabase.GUIDToAssetPath(animators[0]);
        avatar.animationController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animatorPath);


        //Expressions
        testUMA.AddComponent<UMAExpressions>();

        //Speech
        AudioSource audioSource = testUMA.AddComponent<AudioSource>();
        string[] testAudios = AssetDatabase.FindAssets("UMATestAudio");
        string testAudiosPath = AssetDatabase.GUIDToAssetPath(testAudios[0]);
        audioSource.clip= AssetDatabase.LoadAssetAtPath<AudioClip>(testAudiosPath);
        audioSource.playOnAwake = false;
        testUMA.AddComponent<NPCSimpleLipSync>();


    }
}
