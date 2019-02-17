
using System.Drawing;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace Utils
{
    public class QRCode
    {
        QRCodeInfoTable qrCodeInfoTable = new QRCodeInfoTable();

        public Image CreateFixSizeCodeImage(string qrText, float fixWidth, float fixHeight)
        {
            int i = 0;
            Image upImage = null;
            Image nextImage = null;

            while (true)
            {
                if(upImage != null)
                    upImage.Dispose();

                upImage = nextImage;
                nextImage = CreateCodeImage(qrText, ++i);

                if(nextImage.Width >= fixWidth || 
                    nextImage.Height >= fixHeight)
                {
                    return (upImage != null ? upImage : nextImage);
                }
            }
        }


        public Image CreateCodeImage(string qrText, int qrCodeScale)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(qrText);

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            int ver = qrCodeInfoTable.SearchBestInfoCapacityVersion(buffer.Length, 1);

            qrCodeEncoder.QRCodeScale = qrCodeScale;
            qrCodeEncoder.QRCodeVersion = ver;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Image image = qrCodeEncoder.Encode(qrText, Encoding.UTF8);
            return image;
        }
    }
}
