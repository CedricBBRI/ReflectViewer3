using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Reflect;
using UnityEngine.UI;
using System.Reflection;

[AddComponentMenu("Reflect/Template/Project View Info Panel")]
public class ReflectProjectViewInfoPanel : MonoBehaviour
{
    [Tooltip("Root Transform to look for POI objects.")]
    [SerializeField] Transform syncManagerRoot = default;

    [SerializeField] ScrollRect groupsScrollView = default;
    [SerializeField] ScrollRect keysScrollView = default;
    [SerializeField] ScrollRect valuesScrollView = default;

    [SerializeField] Button listItemButtonPrefab = default;

    Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> metadatas = new Dictionary<string, List<string>>();

    MetadataTopMenu metadataTopMenu;
    MethodInfo mInfo;

    private void Awake()
    {
        metadataTopMenu = FindObjectOfType<MetadataTopMenu>();
        mInfo = typeof(MetadataTopMenu).GetMethod("ActivateMenuItem", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private void OnEnable()
    {
        UpdateInfo();
    }

    void UpdateInfo()
    {
        if (syncManagerRoot == null)
            syncManagerRoot = FindObjectOfType<SyncManager>()?.syncRoot;

        if (syncManagerRoot == null)
            return;

        foreach (Metadata md in syncManagerRoot.GetComponentsInChildren<Metadata>())
        {
            foreach (KeyValuePair<string, Metadata.Parameter> kvp in md.parameters.dictionary)
            {
                if (!metadatas.ContainsKey(kvp.Key))
                    metadatas.Add(kvp.Key, new List<string>());

                if (!metadatas[kvp.Key].Contains(kvp.Value.value))
                    metadatas[kvp.Key].Add(kvp.Value.value);

                if (!groups.ContainsKey(kvp.Value.group))
                    groups.Add(kvp.Value.group, new List<string>());

                if (!groups[kvp.Value.group].Contains(kvp.Key))
                    groups[kvp.Value.group].Add(kvp.Key);
            }
        }

        UpdateGroupsUI();
        UpdateKeysUI();
        UpdateValuesUI();
    }

    void UpdateGroupsUI()
    {
        ClearList(groupsScrollView);

        foreach (KeyValuePair <string, List<string>> kvp in groups)
            AddListItem(groupsScrollView, kvp.Key, () => UpdateKeysUI(kvp.Key));
    }

    void UpdateKeysUI (string group = "")
    {
        ClearList(keysScrollView);

        if (group == string.Empty)
            return;

        foreach (string key in groups[group])
            AddListItem(keysScrollView, key, () => UpdateValuesUI(key));
    }

    void UpdateValuesUI(string key = "")
    {
        ClearList(valuesScrollView);

        if (key == string.Empty)
            return;

        foreach (string val in metadatas[key])
        {
            AddListItem(valuesScrollView, val, () =>
            {
                Debug.Log("DO WHAT YOU WANT WITH THAT VALUE HERE"); // <- Do you own thing here

                // Here using Reflection to call the Metadata Top Menu and filter the specific Key/Value pair.
                mInfo.Invoke(metadataTopMenu, new object[2] { key, val });
            });
        }
    }

    void AddListItem (ScrollRect list, string title, UnityAction action)
    {
        var item = Instantiate(listItemButtonPrefab, list.content);
        item.GetComponentInChildren<Text>().text = title;
        item.onClick.AddListener(action);
    }

    void ClearList(ScrollRect list)
    {
        foreach (Transform child in list.content)
            Destroy(child.gameObject);
    }
}