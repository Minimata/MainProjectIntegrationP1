using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothRemoteControl
{
    class TrameSender
    {
        public TrameSender(string trame_unicode, BluetoothZeuGroupeLib.BluetoothClientModule BlModule)
        {
            // Encodage
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Bytes Unicode
            byte[] unicodeBytes = unicode.GetBytes(trame_unicode);

            // Bytes ASCII
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // String en ASCII
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string trame_ascii = new string(asciiChars);

            // Vérifie ASCII
            if (IsASCII(trame_ascii))
            {
               
                // Code (77, 88, 99)
                string trame_code = trame_ascii.Substring(0, 2);

                // Code conduite (88VVRR), vérifie si 6 CHIFFRES
                if (trame_code == "88" && trame_ascii.Length == 6)
                {
                    // Vitesse (VV)
                    string speed = trame_ascii.Substring(2, 2);
                    // Rotation (RR)
                    string rotation = trame_ascii.Substring(4, 2);

                    // Vérifie tranche 0-99
                    if ((Int32.Parse(speed) >= 0 && Int32.Parse(speed) <= 99) && (Int32.Parse(rotation) >= 0 && Int32.Parse(rotation) <= 99))
                    {
                        BlModule.sendToPairedRobot(trame_ascii);
                    }
                }

                // Code 99 et 77
                else if (trame_code == "77" || trame_code == "99")
                    BlModule.sendToPairedRobot(trame_code);

                // Trame invalide
                else
                    Console.WriteLine("Erreur trame");

            }

            else
            {
                Console.WriteLine("Erreur ASCII");
            }
        }

        // Vérifie si ASCII
        public bool IsASCII(string value)
        {
            return Encoding.ASCII.GetByteCount(value) == value.Length;
        }

    }
}
