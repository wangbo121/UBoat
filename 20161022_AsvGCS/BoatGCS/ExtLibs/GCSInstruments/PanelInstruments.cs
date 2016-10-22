using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GCSInstruments
{
    public partial class PanelInstruments : UserControl
    {
        public PanelInstruments()
        {
            InitializeComponent();
        }

        public void SetValue(float speed, float height, float pitch, float roll)
        {
            _ctlPlateSpeed.SetValue(speed);
            _ctlPlateHeight.SetValue(height);
            _ctlStance.SetValue(pitch, roll);
        }
    }
}
