public sealed class LoadingPanelPresenter : UIPresenterBase {
    protected override void OnOpen(UIOpenDataBase openData) {
        LoadingPanelOpenData loadingOpenData = openData as LoadingPanelOpenData;
        if (loadingOpenData == null) {
            return;
        }

        LoadingPanelUIState state = new LoadingPanelUIState();
        state.Title = loadingOpenData.Title;
        state.StepText = loadingOpenData.StepText;
        state.Progress = loadingOpenData.Progress;
        Refresh(state);
    }
}
