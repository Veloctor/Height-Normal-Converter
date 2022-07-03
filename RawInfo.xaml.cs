using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HeightNormalConverter
{
    /// <summary>
    /// RawInfo.xaml 的交互逻辑
    /// </summary>
    public partial class RawInfoDialog : Window
    {
        public nint FileSize;
        public nint PredictSize;
        public int ImageWidth;
        public int ImageHeight;
        public int ChannelByteDepth;
        public int HeaderSize;
        public byte ChannelCount;

        public RawInfoDialog(nint fileSize,string fileName)
        {
            InitializeComponent();
            ChannelCount = byte.Parse(ChannelCountTextBox.Text);
            FileSize = fileSize;
            FileNameTextBlock.Text = fileName;
            FileSizeLabel.Content = $"实际文件大小:{fileSize}B";
            //文件名和文件尺寸
            RefreshFileSize();
        }
        private void ChannelCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!IsInitialized) return;
            if(byte.TryParse(ChannelCountTextBox.Text, out byte count))
            {
                if(count == 1)
                {
                    ChannelCount = count;
                }
                else if (count is > 1 and < 56)
                {
                    ChannelCount = count;
                }
                RefreshFileSize();
            }
        }

        private void ChannelCountTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!byte.TryParse(ChannelCountTextBox.Text, out byte count) || count is 0 or > 55)
            {
                MessageBox.Show("通道数必须是大于等于1且小于等于55的数字.已填入前一个数.");
                ChannelCountTextBox.Text = ChannelCount.ToString();
            }
        }
        void RefreshFileSize()
        {
            if (!IsInitialized) return;
            if(int.TryParse(HeightTextBox.Text,out ImageHeight)&&
                int.TryParse(WidthTextBox.Text,out ImageWidth)&&
                int.TryParse(HeaderSizeTextBox.Text,out HeaderSize))
            {
                if ((bool)ChannelBitDepth8.IsChecked) ChannelByteDepth = 1;
                else if ((bool)ChannelBitDepth16.IsChecked) ChannelByteDepth = 2;
                else ChannelByteDepth = 4;
                nint predictSize = (nint)ImageWidth * ImageHeight * ChannelByteDepth * ChannelCount + HeaderSize;
                DescribedSizeLabel.Content = $"描述信息大小:{predictSize}B";
                if (FileSize == predictSize)
                {
                    ConfirmButton.IsEnabled = true;
                }
                else
                {
                    ConfirmButton.IsEnabled = false;
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //返回:宽度,高度,通道深度,通道数量,交错排列,大小端
            DialogResult = true;
            Close();
        }

        private void RefreshFileSize(object sender, TextChangedEventArgs e)
        {
            RefreshFileSize();
        }

        private void RefreshFileSize(object sender, RoutedEventArgs e)
        {
            RefreshFileSize();
        }
    }
}
