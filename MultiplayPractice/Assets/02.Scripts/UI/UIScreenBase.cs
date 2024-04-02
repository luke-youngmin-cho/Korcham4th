namespace MP.UI
{
    public class UIScreenBase : UIBase
    {
        protected override void Awake()
        {
            base.Awake();

            UIManager.instance.RegisterScreen(this);
        }

        public override void Show()
        {
            base.Show();

            UIManager.instance.SetScreen(this);
        }
    }
}
