using System;

/// <summary>
/// 单个窗口的静态配置。
/// 用于描述层级、缓存策略、打开模式等运行规则。
/// </summary>
public sealed class UIWindowConfig {
    public string UIId;
    public string PrefabKey;
    public UILayer Layer;
    public UICacheMode CacheMode;
    public UIOpenMode OpenMode;
    public bool IsFullScreen;
    public bool UseBackStack;
    public bool BlockRaycast;
    public Type PanelType;
}
