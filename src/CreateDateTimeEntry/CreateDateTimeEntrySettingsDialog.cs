using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateDateTimeEntry
{
  public partial class CreateDateTimeEntrySettingsDialog : Form
  {
    private CreateDateTimeEntrySettings _subject = null;

    public CreateDateTimeEntrySettingsDialog()
    {
      InitializeComponent();
    }

    public CreateDateTimeEntrySettings Subject
    {
      get => _subject;
      set
      {
        _subject = value;
        propertyGrid1.SelectedObject = _subject;
      }
    }
  }
}
