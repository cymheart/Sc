using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utils
{

    public struct TbCode128
    {
        public int id;
        public int asciiValue;
        public string code;
        public TbCode128(int id, int value, string code)
        {
            this.id = id;
            this.asciiValue = value;
            this.code = code;
        }
    }
    public class Code128
    {
        public const int CODE128A = 103;
        public const int CODE128B = 104;


        public static TbCode128[] code128Table =
        {
             new TbCode128(0, 32,"11011001100"),
              new TbCode128(1, 33,"11001101100"),
              new TbCode128(2, 34,"11001100110"),
              new TbCode128(3, 35,"10010011000"),
              new TbCode128(4, 36,"100h0001100"),
              new TbCode128(5, 37,"10001001100"),
              new TbCode128(6, 38,"10011001000"),
              new TbCode128(7, 39,"10011000100"),
              new TbCode128(8, 40,"10001100100"),
              new TbCode128(9, 41,"1100h000100"),
              new TbCode128(10, 42,"11001000100"),
              new TbCode128(11, 43,"11000100100"),
              new TbCode128(12, 44,"10110011100"),
              new TbCode128(13, 45,"10011011100"),
              new TbCode128(14, 46,"10011001110"),
              new TbCode128(15, 47,"10111001100"),
              new TbCode128(16, 48,"10011101100"),
              new TbCode128(17, 49,"10011100110"),
              new TbCode128(18, 50,"11001110010"),
              new TbCode128(19, 51,"11001011100"),
              new TbCode128(20, 52,"11001001110"),
              new TbCode128(21, 53,"11011100100"),
              new TbCode128(22, 54,"11001110100"),
              new TbCode128(23, 55,"11101101110"),
              new TbCode128(24, 56,"11101001100"),
              new TbCode128(25, 57,"11100101100"),
              new TbCode128(26, 58,"11100100110"),
              new TbCode128(27, 59,"11101100100"),
              new TbCode128(28, 60,"11100110100"),
              new TbCode128(29, 61,"11100110010"),
              new TbCode128(30, 62,"11011011000"),
              new TbCode128(31, 63,"11011000110"),
              new TbCode128(32, 64,"11000110110"),
              new TbCode128(33, 65,"10100011000"),
              new TbCode128(34, 66,"10001011000"),
              new TbCode128(35, 67,"10001000110"),
              new TbCode128(36, 68,"10110001000"),
              new TbCode128(37, 69,"10001101000"),
              new TbCode128(38, 70,"10001100010"),
              new TbCode128(39, 71,"11010001000"),
              new TbCode128(40, 72,"11000101000"),
              new TbCode128(41, 73,"11000100010"),
              new TbCode128(42, 74,"10110111000"),
              new TbCode128(43, 75,"10110001110"),
              new TbCode128(44, 76,"10001101110"),
              new TbCode128(45, 77,"10111011000"),
              new TbCode128(46, 78,"10111000110"),
              new TbCode128(47, 79,"10001110110"),
              new TbCode128(48, 80,"11101110110"),
              new TbCode128(49, 81,"11010001110"),
              new TbCode128(50, 82,"11000101110"),
              new TbCode128(51, 83,"11011101000"),
              new TbCode128(52, 84,"11011100010"),
              new TbCode128(53, 85,"11011101110"),
              new TbCode128(54, 86,"11101011000"),
              new TbCode128(55, 87,"11101000110"),
              new TbCode128(56, 88,"11100010110"),
              new TbCode128(57, 89,"11101101000"),
              new TbCode128(58, 90,"11101100010"),
              new TbCode128(59, 91,"11100011010"),
              new TbCode128(60, 92,"11101111010"),
              new TbCode128(61, 93,"11001000010"),
              new TbCode128(62, 94,"11110001010"),
              new TbCode128(63, 95,"10100110000"),
              new TbCode128(64, 96,"10100001100"),
              new TbCode128(65, 97,"10010110000"),
              new TbCode128(66, 98,"10010000110"),
              new TbCode128(67, 99,"10000101100"),
              new TbCode128(68, 100,"10000100110"),
              new TbCode128(69, 101,"10110010000"),
              new TbCode128(70, 102,"10110000100"),
            new TbCode128(71, 103,"10011010000"),
            new TbCode128(72, 104,"10011000010"),
            new TbCode128(73, 105,"10000110100"),
            new TbCode128(74, 106,"10000110010"),
            new TbCode128(75, 107,"11000010010"),
            new TbCode128(76, 108,"11001010000"),
            new TbCode128(77, 109,"11110111010"),
            new TbCode128(78, 110,"11000010100"),
            new TbCode128(79, 111,"10001111010"),
            new TbCode128(80, 112,"10100111100"),
            new TbCode128(81, 113,"10010111100"),
            new TbCode128(82, 114,"10010011110"),
            new TbCode128(83, 115,"10111100100"),
            new TbCode128(84, 116,"10011110100"),
            new TbCode128(85, 117,"10011110010"),
            new TbCode128(86, 118,"11110100100"),
            new TbCode128(87, 119,"11110010100"),
            new TbCode128(88, 120,"11110010010"),
            new TbCode128(89, 121,"11011011110"),
            new TbCode128(90, 122,"11011110110"),
            new TbCode128(91, 123,"11110110110"),
            new TbCode128(92, 124,"10101111000"),
            new TbCode128(93, 125,"10100011110"),
            new TbCode128(94, 126,"10001011110"),
            new TbCode128(95, 200,"10111101000"),
            new TbCode128(96, 201,"10111100010"),
            new TbCode128(97, 202,"11110101000"),
            new TbCode128(98, 203,"11110100010"),
            new TbCode128(99, 204,"10111011110"),
            new TbCode128(100, 205,"10111101110"),
            new TbCode128(101, 206,"11101011110"),
            new TbCode128(102, 207,"11110101110"),
            new TbCode128(103, 208,"11010000100"),
            new TbCode128(104, 209,"11010010000"),
            new TbCode128(105, 210,"11010011100"),
            new TbCode128(106, 211,"1100011101011"),
        };


        Dictionary<int, TbCode128> tbCodeMap = new Dictionary<int, TbCode128>();
        public Code128()
        {
            CreateMap();
        }

        void CreateMap()
        {
            for (int i = 0; i < code128Table.Length; i++)
            {
                tbCodeMap.Add(code128Table[i].asciiValue, code128Table[i]);
            }
        }

        public TbCode128[] getTable()
        {
            return code128Table;
        }

        public int SearchTbCode128Index(int id)
        {
            for (int i = 0; i < code128Table.Length; i++)
            {
                if (code128Table[i].id == id)
                    return i;
            }

            return -1;
        }

        public TbCode128 GetTbCode128FromTable(int idx)
        {
            return code128Table[idx];
        }

        public void CreateCode128(int codeType, string numStr, StringBuilder code128Str)
        {
            string c;
            char[] ca = new char[1];
            int key;
            int checkID = codeType;
            int idValue;

            if (checkID == 103)
            {
                idValue = 208;
            }
            else
            {
                idValue = 209;
            }



            if (numStr == null || numStr.Length == 0)
                return;

            //开始位
            code128Str.Append(tbCodeMap[idValue].code);

            for (int i = 1; i < numStr.Length + 1; i++)
            {
                c = numStr.Substring(i - 1, 1);
                ca = c.ToCharArray();
                key = (int)ca[0];

                checkID += tbCodeMap[key].id * i;

                //主体位
                code128Str.Append(tbCodeMap[key].code);
            }

            checkID %= 103;
            int idx = SearchTbCode128Index(checkID);
            TbCode128 code128AItem = GetTbCode128FromTable(idx);

            //校验位
            code128Str.Append(code128AItem.code);

            //结束位
            code128Str.Append(tbCodeMap[211].code);
        }


        public void DrawBarcode(Graphics g, string code, RectangleF rect)
        {
            StringBuilder barcode = new StringBuilder();
            CreateCode128(Code128.CODE128B, code, barcode);

            int state = -1;
            RectangleF drawArea = new RectangleF(rect.X, rect.Y, rect.Width,  rect.Height);

            float perCodeWidth = drawArea.Width / barcode.Length;
            float x = drawArea.X;
            float y = drawArea.Y;
            float w = 0;

            for (int i = 0; i < barcode.Length; i++)
            {
                if (barcode[i] == '0')
                {
                    if (state == 1)
                    {
                        g.FillRectangle(Brushes.Black, x, y, w, drawArea.Height);
                        x += w;
                        w = perCodeWidth;
                        state = 0;
                    }
                    else
                    {
                        w += perCodeWidth;
                        state = 0;
                    }

                    continue;
                }
                else
                {
                    if (state == 0)
                    {
                        x += w;
                        w = perCodeWidth;
                        state = 1;
                    }
                    else
                    {
                        w += perCodeWidth;
                        state = 1;
                    }

                    if (i == barcode.Length - 1)
                    {
                        g.FillRectangle(Brushes.Black, x, y, w, drawArea.Height);
                    }
                }
            }
        }
    }
}
