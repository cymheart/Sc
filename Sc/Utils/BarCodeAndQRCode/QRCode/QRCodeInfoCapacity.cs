
using System.Collections.Generic;

namespace Utils
{
    class QRCodeInfoCapacity
    {
        public int version;
        public int eccLevel;
        public int byteNum;

        public QRCodeInfoCapacity(int version, int eccLevel, int byteNum)
        {
            this.version = version;
            this.eccLevel = eccLevel;
            this.byteNum = byteNum;
        }
    }

    class QRCodeInfoTable
    {
        public static QRCodeInfoCapacity[] qrCodeInfoTable =
        {

            //1
             new QRCodeInfoCapacity(1, 0,17),
             new QRCodeInfoCapacity(1, 1,14),
             new QRCodeInfoCapacity(1, 2,11),
             new QRCodeInfoCapacity(1, 3,7),

             //2
             new QRCodeInfoCapacity(2, 0,32),
             new QRCodeInfoCapacity(2, 1,26),
             new QRCodeInfoCapacity(2, 2,20),
             new QRCodeInfoCapacity(2, 3,14),

             //3
             new QRCodeInfoCapacity(3, 0,53),
             new QRCodeInfoCapacity(3, 1,42),
             new QRCodeInfoCapacity(3, 2,32),
             new QRCodeInfoCapacity(3, 3,24),

             //4
             new QRCodeInfoCapacity(4, 0,78),
             new QRCodeInfoCapacity(4, 1,62),
             new QRCodeInfoCapacity(4, 2,46),
             new QRCodeInfoCapacity(4, 3,34),

             //5
             new QRCodeInfoCapacity(5, 0,106),
             new QRCodeInfoCapacity(5, 1,84),
             new QRCodeInfoCapacity(5, 2,60),
             new QRCodeInfoCapacity(5, 3,44),

             //6
             new QRCodeInfoCapacity(6, 0,134),
             new QRCodeInfoCapacity(6, 1,106),
             new QRCodeInfoCapacity(6, 2,74),
             new QRCodeInfoCapacity(6, 3,58),

             //7
             new QRCodeInfoCapacity(7, 0,154),
             new QRCodeInfoCapacity(7, 1,122),
             new QRCodeInfoCapacity(7, 2,86),
             new QRCodeInfoCapacity(7, 3,64),

             //8
             new QRCodeInfoCapacity(8, 0,192),
             new QRCodeInfoCapacity(8, 1,152),
             new QRCodeInfoCapacity(8, 2,108),
             new QRCodeInfoCapacity(8, 3,84),

             //9
             new QRCodeInfoCapacity(9, 0,230),
             new QRCodeInfoCapacity(9, 1,180),
             new QRCodeInfoCapacity(9, 2,130),
             new QRCodeInfoCapacity(9, 3,98),

             //10
             new QRCodeInfoCapacity(10, 0,271),
             new QRCodeInfoCapacity(10, 1,213),
             new QRCodeInfoCapacity(10, 2,151),
             new QRCodeInfoCapacity(10, 3,119),

             //11
             new QRCodeInfoCapacity(11, 0,321),
             new QRCodeInfoCapacity(11, 1,251),
             new QRCodeInfoCapacity(11, 2,177),
             new QRCodeInfoCapacity(11, 3,137),

             //12
             new QRCodeInfoCapacity(12, 0,367),
             new QRCodeInfoCapacity(12, 1,287),
             new QRCodeInfoCapacity(12, 2,203),
             new QRCodeInfoCapacity(12, 3,155),

             //13
             new QRCodeInfoCapacity(13, 0,425),
             new QRCodeInfoCapacity(13, 1,331),
             new QRCodeInfoCapacity(13, 2,241),
             new QRCodeInfoCapacity(13, 3,177),

             //14
             new QRCodeInfoCapacity(14, 0,458),
             new QRCodeInfoCapacity(14, 1,362),
             new QRCodeInfoCapacity(14, 2,258),
             new QRCodeInfoCapacity(14, 3,194),

             //15
             new QRCodeInfoCapacity(15, 0,520),
             new QRCodeInfoCapacity(15, 1,412),
             new QRCodeInfoCapacity(15, 2,292),
             new QRCodeInfoCapacity(15, 3,220),

              //16
             new QRCodeInfoCapacity(16, 0,586),
             new QRCodeInfoCapacity(16, 1,450),
             new QRCodeInfoCapacity(16, 2,322),
             new QRCodeInfoCapacity(16, 3,250),

              //17
             new QRCodeInfoCapacity(17, 0,644),
             new QRCodeInfoCapacity(17, 1,504),
             new QRCodeInfoCapacity(17, 2,364),
             new QRCodeInfoCapacity(17, 3,280),

              //18
             new QRCodeInfoCapacity(18, 0,718),
             new QRCodeInfoCapacity(18, 1,560),
             new QRCodeInfoCapacity(18, 2,394),
             new QRCodeInfoCapacity(18, 3,310),

              //19
             new QRCodeInfoCapacity(19, 0,792),
             new QRCodeInfoCapacity(19, 1,624),
             new QRCodeInfoCapacity(19, 2,442),
             new QRCodeInfoCapacity(19, 3,338),

              //20
             new QRCodeInfoCapacity(20, 0,858),
             new QRCodeInfoCapacity(20, 1,666),
             new QRCodeInfoCapacity(20, 2,482),
             new QRCodeInfoCapacity(20, 3,382),
        };


        public int SearchBestInfoCapacityVersion(int byteNum, int eccLevel)
        {
            List<QRCodeInfoCapacity> list = new List<QRCodeInfoCapacity>();

            for (int i = 0; i < qrCodeInfoTable.Length; i++)
            {
                if (qrCodeInfoTable[i].byteNum > byteNum)
                {
                    list.Add(qrCodeInfoTable[i]);
                }
            }


            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].eccLevel >= eccLevel)
                {
                    return list[i].version;
                }
            }

            return 40;
        }
    }
}
