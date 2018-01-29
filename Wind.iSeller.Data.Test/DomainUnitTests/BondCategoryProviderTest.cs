using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Domain;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.DomainUnitTests
{
    [TestClass]
    public class BondCategoryProviderTest : TestBase<DataTestModule>
    {
        private readonly BondCategoryProvider provider;

        public BondCategoryProviderTest()
        {
            this.provider = Resolve<BondCategoryProvider>();
        }

        [TestMethod]
        public virtual void GetBondCategorys()
        {
            var result = this.provider.GetBondCategorys(BondType.CreditBond, new string[] {
                "短融", "中票", "其他"
            });
            string[] shouldBe = new string[] {
                "短期融资券", "超短期融资券",
                "中期票据",
                "国债", "地方政府债", "央行票据", "同业存单", "金融债", "定向工具", "国际机构债", "政府支持机构债", "资产支持债券", "可转债", "可交换债", "可分离转债存债", "私募债"
            };

            Assert.AreEqual(shouldBe.Length, result.Length);
            CollectionAssert.AreEquivalent(shouldBe, result);
        }
    }
}
