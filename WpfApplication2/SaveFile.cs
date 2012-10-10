using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ForkliftManager
{
    class SaveFile
    {
        public SaveFile()
        {
            
        }
        public void Save(List<Card> cards)
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Create))
                {
                    BinaryFormatter bin1 = new BinaryFormatter();
                    bin1.Serialize(stream, cards);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }
        public List<Card> Open()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Open))
                {
                    BinaryFormatter bin2 = new BinaryFormatter();
                    List<Card> cards2 = new List<Card>();
                    cards2 = (List<Card>)bin2.Deserialize(stream);
                    stream.Close();
                    stream.Dispose();
                    return cards2;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return new List<Card>();
        }
    }
}
