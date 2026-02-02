using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class ItemDataEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Item Data Editor")]
    private static void OpenWindow()
    {
        GetWindow<ItemDataEditor>().Show();
    }

    private CreateNewItemData createNewItemData;
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (createNewItemData != null)
            DestroyImmediate(createNewItemData.itemData);
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();

        createNewItemData = new CreateNewItemData();
        tree.Add("Create New", createNewItemData);
        tree.AddAllAssetsAtPath("Item Data", "Assets/Scripts/Items/ItemDatas", typeof(ItemData));

        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();

        GUILayout.FlexibleSpace();

        if (SirenixEditorGUI.ToolbarButton("Delete Current"))
        {
            ItemData asset = selected.SelectedValue as ItemData;
            string path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        SirenixEditorGUI.EndHorizontalToolbar();
    }
    public class CreateNewItemData
    {
        public CreateNewItemData()
        {
            itemData = ScriptableObject.CreateInstance<ItemData>();
        }
        public string dataName = "NewData";
        [BoxGroup("Setting")]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ItemData itemData;

        [Button("Add New Data")]
        private void CreateNewData()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Scripts/Items/ItemDatas/" + dataName + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ItemData>();
        }
    }
}
