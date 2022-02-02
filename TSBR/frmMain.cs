namespace TSBR
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MessageBox.Show("Own3d");

            Application.Exit();
        }
    }
}