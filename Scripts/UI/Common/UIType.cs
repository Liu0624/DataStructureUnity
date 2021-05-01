namespace WJF_CodeLibrary.UIFramework
{
    public class UIType
    {
        public UIPanelType panelType;
        public UIShowType showType;
        public UIMaskType maskType;

        public UIType()
        {
            panelType = UIPanelType.Normal;
            showType = UIShowType.Irrelevant;
            maskType = UIMaskType.Normal;
        }
    }
}