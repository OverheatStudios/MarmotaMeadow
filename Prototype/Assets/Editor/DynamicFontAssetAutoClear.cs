// code stolen from https://discussions.unity.com/t/tmpro-dynamic-font-asset-constantly-changes-in-source-control/868941/19

// TextMeshPro dynamic font assets have a very annoying habit of saving their dynamically generated binary data in the
// same text file as their configuration data. This causes massive headaches for version control.
//
// This script addresses the above issue. It runs whenever any assets in the project are about to be saved. If any of
// those assets are a TMP dynamic font asset, they will have their dynamically generated data cleared before they are
// saved, which prevents that data from ever polluting the version control.
//
// For more information, see this thread: https://discussions.unity.com/t/868941


#if UNITY_EDITOR

using System;
using System.Drawing.Printing;
using TMPro;
using UnityEditor;
using UnityEngine;

internal class DynamicFontAssetAutoClear : AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        try
        {
            foreach (string path in paths)
            {
            //    Debug.Log(path);
                if (AssetDatabase.LoadAssetAtPath(path, typeof(TMP_FontAsset)) is TMP_FontAsset fontAsset)
                {
                //    Debug.Log("is font");
                    if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                    {
                     //   Debug.Log("Clearing font asset data at " + path);
                        fontAsset.ClearFontAssetData(setAtlasSizeToZero: true);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Something went wrong while clearing dynamic font data. For more info look at log message above.");
        }

        return paths;
    }
}

#endif