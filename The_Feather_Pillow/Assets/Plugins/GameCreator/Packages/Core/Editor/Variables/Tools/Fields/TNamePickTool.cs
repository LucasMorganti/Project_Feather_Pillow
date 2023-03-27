using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Editor.Common.ID;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TNamePickTool : VisualElement
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/NamePickTool";

        protected const string NAME_ROOT_NAME = "GC-NamePickTool-Name";
        protected const string NAME_DROPDOWN = "GC-NamePickTool-Dropdown";

        protected static readonly IIcon ICON_DROPDOWN = new IconArrowDropDown(ColorTheme.Type.TextLight);
        
        // MEMBERS: -------------------------------------------------------------------------------

        protected readonly SerializedProperty m_Property;
        
        private readonly SerializedProperty m_PropertyVariable;
        protected readonly SerializedProperty m_PropertyName;

        private readonly ObjectField m_Asset;
        
        protected readonly bool m_AllowAny;
        protected readonly bool m_AllowCast;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected IdString TypeID { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TNamePickTool(ObjectField asset, SerializedProperty property, 
            bool allowAny, bool allowCast, IdString typeID)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets)
            {
                this.styleSheets.Add(styleSheet);
            }
            
            asset.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);
            
            this.m_Property = property;
            this.m_Property.serializedObject.Update();
            
            this.m_PropertyVariable = property.FindPropertyRelative("m_Variable");
            this.m_PropertyName = property.FindPropertyRelative($"m_Name.{IdStringDrawer.NAME_STRING}");

            this.m_AllowAny = allowAny;
            this.m_AllowCast = allowCast;
            
            this.TypeID = typeID;
            this.m_Asset = asset;
            
            asset.UnregisterValueChangedCallback(this.OnChangeAsset);
            asset.RegisterValueChangedCallback(this.OnChangeAsset);
            
            this.RefreshPickList(this.m_PropertyVariable.objectReferenceValue);
        }

        private void OnChangeAsset(ChangeEvent<Object> changeEvent)
        {
            this.RefreshPickList(changeEvent.newValue);
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void RefreshPickList(Object asset)
        {
            this.Clear();
        }

        protected Dictionary<string, bool> FilterNames(NameList names)
        {
            Dictionary<string, bool> list = new Dictionary<string, bool> {{ string.Empty, false }};
            
            for (int i = 0; i < names?.Length; ++i)
            {
                NameVariable nameVariable = names.Get(i);
                list[nameVariable.Name] = false;
                
                if (this.m_AllowAny)
                {
                    bool isNull = nameVariable.TypeID.Hash == ValueNull.TYPE_ID.Hash;
                    list[nameVariable.Name] = !isNull;
                    continue;
                }

                if (nameVariable.TypeID.Hash == this.TypeID.Hash)
                {
                    list[nameVariable.Name] = true;
                    continue;
                } 
                
                if (this.m_AllowCast && this.TypeID.Hash == ValueString.TYPE_ID.Hash)
                {
                    bool isNull = nameVariable.TypeID.Hash == ValueNull.TYPE_ID.Hash;
                    list[nameVariable.Name] = !isNull;
                    continue;
                }

                list[nameVariable.Name] = false;
            }

            return list;
        }
    }
}