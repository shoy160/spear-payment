using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Acb.Payment.Tests
{
    [TestClass]
    public class ChannelServiceTest : DTest
    {
        private readonly IChannelContract _channelContract;
        private readonly string _testId = "882752fd-e41c-c675-bb9b-08d62dfc7ea2";

        public ChannelServiceTest()
        {
            _channelContract = Resolve<IChannelContract>();
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            //var result = await _channelContract.CreateAsync(new ChannelInputDto
            //{
            //    Name = "默认支付宝",
            //    Remark = "默认的支付宝渠道",
            //    Mode = TradeMode.Alipay,
            //    AppId = "2017102609532728",
            //    Config = new
            //    {
            //        alipayPublicKey =
            //            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhys8PYmv2C9u8b0Z3c6gaFzeU7H9O/OZDDjmrj7daEbn4DGfQLI/f2BNdD/A4U/6AfxwIecQ4EJszW68O5Q7TDq+6exLlA5cH0czGHtN4aJ1B4FMcxzyfaf9Nv6n6H4K1PvCiLStUZ/MyocsbBXV5j3XVRgPm68ktRq5v2Ng0MtQ9pqhcyD8ux9n31nxIejJT631N4r4KTn4bOQy5sw0ZK5N/f/i7GjcjxcyhjK1SonZXBFa3rPW2M/a5Rj03TB8b7B37v6k6/mjt58DA2k2bE8qqBYRHZZMLmNmWERd4kWbIuc0pK5psOzJgEaLvBEcKW61WSfrF50gS5B+EQV/JQIDAQAB",
            //        appId = "2017102609532728",
            //        notifyUrl = "http://iapp.i-cbao.com/notify/payment",
            //        privateKey =
            //            "MIIEowIBAAKCAQEA3C7iTPHg81Ej12mgisd/XhQGDwIIcnWRofOIRgn26y7MXRAgUz8npkiZr3pE9iXouD4RdJzpjNv4J1sVCkKUAjjZBkOp3OkmxmOGOy0H9DWAcqwQ6ohSJcewBAi7P3IS1Xia6Dh1+Uzy9TijgRxsgfdE10rD5hbBHrAEjecbpK2ReOTzAcoe/XAFSsKgaGeMnKd/j+AGWj/cOcm7bpUjqSjVLTcIr9GcTKeJKCD7QA26+9qhE3eZE3nK0JKIrwVg549rxOfeA64aEU1JiprHGClXwks0THZ76GbGheRmTndUZzhYLNgUWcOuLiDRcEila1hwr1e6axFiq5/fUOL+IwIDAQABAoIBAQCwD3088Zxic1spgHiy/9rEq1Y84e9HNuOAkG9DMeyTGhSnxaFTb4iQ2jSpsCc8fAueZ2Mlz+Kgk7PWJGqqjZo0PKis7aYB0x0CPcqzvspBaBaRmyzbnK2zL/16/FBd5yVQNOQJFDyhX/pWQzfaoZPSSJpvXIVQkplRpIW/wjDEd9jmvFmgZYFLgLhT32MurK0f5dGi2dPj2mwtxBkQ3h/2/Ah1m2CNRfGsP5XaJGEI8TO6x5eXQ1qDISZ/UtU7oLZp3Mdx68Bp7+7Tebv/xCzWuqR7UkZuXaxBZ3PATV/FJpnvYWyYSAWxnp/p9KYgbBsaVjKw8afdSoQreQUu7GsBAoGBAP5lk7LNKxq5+cUI7UNlcH4japeWvjMKiW1SIvYTq5e4jaHaqnYr6OnZezp5wmKlqV4s6UjjmCUECW+KoGYu3tV8Z293Po6cfpKBCtif1WwjUQffIku+puL5mGFxF3C4uSx31tiEsHJanhkESJQwTTva/0CTLbBELj8VY966DZbhAoGBAN2SHApPKVDle1oWOHAwtcXRuPETZa0Z/AlLHHgRWKZaawQuynJPMjGq1nmpajUJrwsDD4DgOyDxRSABLSTzXxC2FBFB0uGExK2rnQ2L3KUFLvBdIzutE0DZSye+rh/CsaKPwLg330PDrvvijudEOyLfvCzP1l0CPLmb6jPdHOmDAoGACGpj0G5pFqFAWfxJnQkmB4Y0aH9cG2Ql16/36BwOeR5p0QjiyrX5JoNDSFPu0kSYrbzemsKsrczMYxw64iZ6yKUs5ssTDrSumzoZmcDv1nv0mkYPZGISMz/+gnqzQ89YkNgGC3OYQrAsurchj4VpiKld+EzmHgajYQLcIuoUuQECgYB1PpZ14hOzyeru7akpvKzDI8ngT2pTIdfbNxK2ej9Vr6zHRtc7i6q823CoajxYGmq0wgbXJFBFi0YPvG+eCOY6Go8d3p3vVekZGSF/BI5aCBche7dkIZ55h0IcourSxZxnr/fDw1TyL78NbII/4DwGpSuW7te42bE2akmQ7iUZbwKBgDYatSgniSgZQmkWvv8VUenHgMaaC91tlhGKZ45oBm2Kcf7xreqqsEjxavX1Ha5OmEtNmXcZCZ3cGOsWgh92ljWrEkflu9o31mnob3Bi6FNGw9iuZuCqEn9esMB1m7aq0Y2CqlVm0ez1r9yfDZatWR+Gh3ZvdfTF2gPiPacaVGgz"
            //    }
            //});
            var result = await _channelContract.CreateAsync(new ChannelInputDto
            {
                Name = "微信(默认)",
                Remark = "默认的微信渠道",
                Mode = PaymentMode.Wechat,
                AppId = "wx615260566af104db",
                Config = new
                {
                    appId = "wx615260566af104db",
                    appSecret = "PlbYvz59xfDBdEzkg5DLX5yDgI7oor6V",
                    key = "PlbYvz59xfDBdEzkg5DLX5yDgI7oor6V",
                    mchId = "1491231192",
                    notifyUrl = "http://iapp.i-cbao.com/notify/payment",
                    sslCertPassword = "",
                    sslCertPath = ""
                }
            });
            Assert.AreNotEqual(result, 0);
            Print(result);
        }

        [TestMethod]
        public async Task DetailAsyncTest()
        {
            var dto = await _channelContract.DetailAsync(_testId);
            Assert.AreNotEqual(dto, null);
            Print(dto);
        }

        [TestMethod]
        public async Task SetAsyncTest()
        {
            var result = await _channelContract.SetAsync(_testId,
                new ChannelInputDto { Types = new[] { PaymentType.App } });
            Assert.AreNotEqual(result, 0);
            Print(result);
        }

        [TestMethod]
        public async Task SetDefaultTest()
        {
            var result = await _channelContract.SetDefaultAsync("f769b7ff-dfa0-c9d6-f012-08d635b8fd8a", true);
            Assert.AreNotEqual(result, 0);
            Print(result);
        }

        [TestMethod]
        public async Task DetailsAsyncTest()
        {
            var dict = await _channelContract.DetailsAsync(new[] { _testId });
            Assert.AreNotEqual(dict, null);
            Print(dict);
        }

        [TestMethod]
        public async Task ListAsyncTest()
        {
            var dtos = await _channelContract.ListAsync(new ChannelListInputDto());
            Assert.AreNotEqual(dtos, null);
            Print(dtos);
        }

        [TestMethod]
        public async Task DetailByAppIdAsyncTest()
        {
            var dto = await _channelContract.DetailByAppIdAsync("wx5dbe9eeb9f246b63");
            Print(dto);
        }

        [TestMethod]
        public void ConvertTest()
        {
            var key = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDRVL/RPP9lNnQGxqrTDVkca1z8xJ8iogt13xweh2JWFp/Wh3F/RYj2GX/5f13uBw8Kx4coW8YmG8DBeQ+teuq8qRZhffy1MFya/WPVXmUWw0STOxPmH3QTJQoWnQ2oivflwdkOI0nmPRDFR1om+S35H8fr9879urlClrNf8NiDLre8AVMUr7OUuHCvbBu66B7Y0VsTL4gAm5N3zOCOMp+EodcF1sFOZz/hOcDKeH6mi2yJ5ZzwiocHaAUu3uYo5ZoWD/82YRgLiit6EwKTts/+aMJP3+YX6rJhP5iHoL2Ugi9D7Gx+4Wu9auiqsPapd6EN9ifcfOOUtm54hO2b6+CTAgMBAAECggEBAMyRk/s5LyWMN4s/r0UdsE4TpgXnaNUBo02HwgUbrUAQUBfouUP1gklu1h/PFs4827hfEXihZ11qlc79LhMNIkVIwPR5mPLA1l8o7d1gqE28elGf1Gx5pIfpFJjQ1r41QNmpvSMR3XBBkRgaCMI5lXH2WvwBaqmPRukKOTWzOwPGaIqiXq1aTvEAzKwiHD+FNzIsJ4beAS227epHChdIjJAMK5xNNRgfJPEc1lPCnQ2yym0y8cguNO0Wsh3MdvxPSWAE/uHVzHMflypeFcjasjbNspFSwpXmXEV7R01gyDhFi/L4dzqsnXSh20kIZIe9Uukx3tN9vxLyifiEtaaCHHECgYEA98oiWZmDDlw08q+ARePk/vJYg3X+uVaoU5DIN7/e3iasz4QjxG8kMd0OMDWoyQqU6JClfN+fgZcBbDUNepO6BJmiRfkXbO4giOthkUlf2ugFSI+F3YOcdTR+Y+32pTvypbY/WvF/TSq5/ZOGRnhCmXvr213JcNXrHRq1QtwFzl0CgYEA2ERkXIXoIB6CZfUNzSbQIFvAFpazgVIRpS9OceRjhUeiXr7845rewSyrgc423zvrrMCbEGB6FwmIP6FNp+Nkl5t4BD8FEY69MJVfD6cee1AfmVEM5ba3lBzrfe/Zo3c6QwCHIIVdnDKeb5mFuHc6uhSA2n/ZkAR5QtpHbaDwG68CgYAKSbEtaStA0GE+Zjz8Kd8bGrLEGoUN6uQoPA2kCupU7OQl5YWapUaqb1QkekXcuvy9vyuvyJUVy1A6zuUVXsbNZBeRsirf0e814Bf38UTykMmPXq4fKVS4pQpOWdXwvFFxweRVgUtYx9pbBeIAUQPq3XNIc1dmwOr3FWHLOdlf3QKBgDrlpU9tSGI27NxHeJK+Vz/4vL9qG0jEyPlrgLmTsWbCs32KFeUGcFO7jpmzR05US0Ko0ZIMNDPB7NEnZLasPuDq/ixp3T+C2BM7GsBwiuSaVYCzigelUymLFrcdcvAJsFw+8I3yxCOdxXgvHzO1hNDYdJ8M4ntOPJfwSjOY+MsRAoGBAM/LpPdZFCyCoYsr9pJfdXTtiajH62ZktY6ANimEeJhtHPY1S2WzdwnluvoG88FinUwpyaRbwRW6VegFv1caW+x2txVLaoVb5y5eH/1e3EFbpaS2DRkIYf+njvp7d7HCHDtMlq8i4HxIUpZhBC9d2g1+2Cf3a9L4jLNN11byicqz";

            var t = RsaKeyConverter.RSAPrivateKeyJava2DotNet(key, "RSA2");
            Print(t);
        }
    }
}
