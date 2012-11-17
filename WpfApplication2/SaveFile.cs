﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ForkliftManager
{

    /*
     * Made By David Barka 17.11.2012
     * davidbarka@gmail.com
     */
    class SaveFile
    {
        public SaveFile()
        { }

        public void Save(List<Card> cards)
        {
            try
            {
                using (Stream stream = File.Open("cards.bin", FileMode.Create))
                {
                    BinaryFormatter bin1 = new BinaryFormatter();
                    bin1.Serialize(stream, cards);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Klarte ikke å lagre cards.bin!");
            }
        }
        public List<Card> Open()
        {
            try
            {
                using (Stream stream = File.Open("cards.bin", FileMode.Open))
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
                System.Windows.MessageBox.Show("Fant ikke cards.bin! Ny fil blir opprettet.");
            }
            return new List<Card>();
        }

        public void Save(List<ServiceHistory> serviceLists)
        {
            try
            {
                using (Stream stream = File.Open("service.bin", FileMode.Create))
                {
                    BinaryFormatter bin1 = new BinaryFormatter();
                    bin1.Serialize(stream, serviceLists);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Klarte ikke å lagre service.bin!");
            }
        }
        public List<ServiceHistory> OpenService()
        {
            try
            {
                using (Stream stream = File.Open("service.bin", FileMode.Open))
                {
                    BinaryFormatter bin2 = new BinaryFormatter();
                    List<ServiceHistory> serviceLists2 = new List<ServiceHistory>();
                    serviceLists2 = (List<ServiceHistory>)bin2.Deserialize(stream);
                    stream.Close();
                    stream.Dispose();
                    return serviceLists2;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fant ikke service.bin! Ny fil blir opprettet.");
            }
            return new List<ServiceHistory>();
        }
    }
}
