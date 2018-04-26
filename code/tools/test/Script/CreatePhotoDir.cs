using System.IO;

namespace Script
{
    public class CreatePhotoDir : BaseClass
    {
        public CreatePhotoDir()
        {
            var haha = new string[]
            {
                "A_18c1礼盒雅韵三件套",
                "B_12c1圣经册水晶面板",
                "C_精美万年历一副",
                "D_60x120c1豪华精美彩框一副",
                "E_照片墙精美一副",
                "F_60x120c1钢化玻璃水晶一副",
                "G_韩式四组合一副",
                "H_36c1精美彩框一副",
                "I_30c1大韩水晶一副",
                "J_三连照片组合一副",
                "K_24c1玻璃水晶一副",
                "L_中古风一副",
                "M_心形精品摆台一副",
                "N_韩丽斯摆台一副",
                "O_双层园摆台一副",
                "P_10c1精美摆台一副",
                "Q_12c1水晶摆台一副",
                "赠送_60x160精美海报一副"
            };

            for (int i = 0; i < haha.Length; i++)
            {
                CreateDirectory(haha[i] + "/");
            }
        }
    }
}