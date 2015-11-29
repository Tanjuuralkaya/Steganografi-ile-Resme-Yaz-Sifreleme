using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganografi_ile_Resim_Sifreleme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void al(List<int> m_renk)
        {
            List<int> karakter_listesi = new List<int>();
            String alinan = textBox1.Text;
            karakter_listesi = metin_donustur(alinan);
            pixel_setle(alinan.Count(), karakter_listesi, m_renk);
        }
        private List<int> renk_pixelini_al(PictureBox a)
        {
            Bitmap bmp = new Bitmap(a.Image);
            List<int> mavi_degerleri_tut = new List<int>();
            //Color.pixrengi = bmp.GetPixel(50, 50);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color renk = bmp.GetPixel(i, j);
                    int mavi = renk.B;
                    int[] donen_binary = degerdonustur(mavi);
                    for (int k = 0; k < donen_binary.Count(); k++)
                    {
                        mavi_degerleri_tut.Add(donen_binary[k]);
                    }

                }
            }
            return mavi_degerleri_tut;
        }
        private int[] degerdonustur(int a)
        {
            int[] binary = { 0, 0, 0, 0, 0, 0, 0, 0 };
            int i = 7;
            do
            {
                int kalan = a % 2;
                binary[i] = kalan;
                a = a / 2;
                i--;
            } while (i >= 0);
            return binary;
        }
        private List<int> metin_donustur(string metin)
        {
            byte[] ascii = System.Text.Encoding.ASCII.GetBytes(metin);
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(metin);

            int[] binary_donen;
            List<int> donen_degeri_tut = new List<int>();
            try
            {
                for (int i = 0; i < ascii.Count(); i++)
                {
                    binary_donen = degerdonustur(ascii[i]);//her ascii karakterini binary ye döndermek için.
                    for (int k = 0; k < 8; k++)
                    {
                        donen_degeri_tut.Add(binary_donen[k]);//karakterin binary değerlerini listeye ekleme.
                    }

                }
            }
            catch (Exception a)
            {
                MessageBox.Show("hata" + a);
            }
            return donen_degeri_tut;
        }
        private void pixel_setle(int liste_boyu, List<int> binary_metin, List<int> binary_renk)
        {
            int[] list = degerdonustur(liste_boyu);
            for (int j = 0; j < list.Count(); j++)
            {
                binary_renk[j] = list[j];
            }
            for (int i = 1; i <= binary_metin.Count(); i++)
            {
                binary_renk[8 * i + 7] = binary_metin[i - 1];//metinin sekiz bitini mavinin ilk sekiz bitine gömdük
                //o yuzden anlamsız bıtten baslarız metni resme gömmeye 
            }
            ciz(decimale_donustur(binary_renk));
        }
        private List<int> decimale_donustur(List<int> binary_mavi)
        {
            List<int> decimal_sayi = new List<int>();
            double dec_sayi = 0.0;
            try
            {

                for (int i = 0; i < binary_mavi.Count(); i += 8)
                {
                    dec_sayi = binary_mavi[i] * Math.Pow(2.0, 7.0);
                    dec_sayi += binary_mavi[i + 1] * Math.Pow(2.0, 6.0);
                    dec_sayi += binary_mavi[i + 2] * Math.Pow(2.0, 5.0);
                    dec_sayi += binary_mavi[i + 3] * Math.Pow(2.0, 4.0);
                    dec_sayi += binary_mavi[i + 4] * Math.Pow(2.0, 3.0);
                    dec_sayi += binary_mavi[i + 5] * Math.Pow(2.0, 2.0);
                    dec_sayi += binary_mavi[i + 6] * Math.Pow(2.0, 1.0);
                    dec_sayi += binary_mavi[i + 7] * Math.Pow(2.0, 0.0);
                    decimal_sayi.Add(Convert.ToInt16(dec_sayi));
                    dec_sayi = 0.0;
                }
            }
            catch (Exception a) { MessageBox.Show(" " + a); }
            return decimal_sayi;
        }
        private void ciz(List<int> yeni_renk)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int k = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    bmp.SetPixel(i, j, Color.FromArgb(pixel.A, pixel.R, pixel.G, yeni_renk[k++]));
                }

            }
            pictureBox2.Image = bmp;

        }
        private void btnYaziGGom_Click(object sender, EventArgs e)
        {
            try
            {

                al(renk_pixelini_al(pictureBox1));
            }
            catch (Exception A)
            {
                MessageBox.Show(A.ToString());
            }
        }
        private List<int> anlamsiz_bitleri_tut(List<int> tum_mavi_bitleri)
        {
            List<int> binary_karakter = new List<int>();

            for (int j = 0; j < 8; j++)
                binary_karakter.Add(tum_mavi_bitleri[j]);

            for (int i = 8; i < decimale_donustur(tum_mavi_bitleri)[0] * 64 + 8; i += 8)
                binary_karakter.Add(tum_mavi_bitleri[i + 7]);
            return binary_karakter;
        }
        private List<char> karakter_donustur(List<int> dec_deger)
        {
            List<char> karakter_listesi = new List<char>();

            for (int i = 1; i <= dec_deger[0]; i++)
            {
                Console.WriteLine("" + dec_deger[i]);
                karakter_listesi.Add(Convert.ToChar(dec_deger[i]));
            }

            return karakter_listesi;
        }
        private void yazdir(List<char> donusen_karakter)
        {
            for (int i = 0; i < donusen_karakter.Count(); i++)
                textBox2.Text = textBox2.Text + donusen_karakter[i].ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            List<int> donen_karakter_bitleri = new List<int>();
            List<char> donen_karakterler = new List<char>();
            donen_karakter_bitleri = anlamsiz_bitleri_tut(renk_pixelini_al(pictureBox2));
            donen_karakterler = karakter_donustur(decimale_donustur(donen_karakter_bitleri));
            yazdir(donen_karakterler);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(@"c:\users\tanju\documents\visual studio 2015\Projects\Steganografi ile Resim Sifreleme\Steganografi ile Resim Sifreleme\img\Wallpaper 1080p (71).jpg");
          
        }
      
    }
}
