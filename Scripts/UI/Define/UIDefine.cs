namespace WJF_CodeLibrary.UIFramework
{
    public enum UIPanelType
    {
        Normal,//普通面板（如主菜单）
        Fixed,//固定面板（如顶部工具面板）
        Popup,//弹出面板（如对话框提示）
    }

    public enum UIShowType
    {
        Relevant,//关联显示（进入UI栈，形成导航链，可做撤销）
        Irrelevant,//非关联显示（开关不影响其他面板）
    }

    public enum UIMaskType
    {
        None,//无遮罩
        Normal,//有遮罩，普通透明度，预设值
    }

    public enum MessageBoxType
    {
       Inform,//消息提示框
        OK,//确定提示框
        ConfirmCancel,//确定取消提示框
    }

    public enum EvidenceConfirmType
    {
        Capture,//截图形式
        Sprite,//精灵图片直接传递
    }

    public partial class UIDefine
    {
        public const string UIPathRoot = "Prefab/WJF/UI/";

        //UI父物体节点常量
        public const string NormalNode = "Normal";
        public const string FixedNode = "Fixed";
        public const string PopupNode = "Popup";

        //UI的prefab路径
        public const string CanvasPrefabPath = UIPathRoot + "Common/Canvas";
        public const string MaskPrefabPath = UIPathRoot + "Common/MaskPanel";

        //遮罩常量
        public const float MaskNormalColorRGB = 255 / 255f;
        public const float MaskNormalColorAlpha = 255 / 255f;
    }
}