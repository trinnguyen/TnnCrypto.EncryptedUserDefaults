using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Xunit;

namespace TnnCrypto.Tests.Tests
{
    public class EncryptedUserDefaultsTests
    {
        private static EncryptedUserDefaults Create()
        {
            return EncryptedUserDefaults.CreateStandardEncryptedUserDefaults();
        }

        [Fact]
        public void Init_UserDefaults_ShouldStandard()
        {
            EncryptedUserDefaults def = EncryptedUserDefaults.CreateStandardEncryptedUserDefaults();
            Assert.Equal(NSUserDefaults.StandardUserDefaults, def.UserDefaults);
        }

        [Theory]
        [InlineData("key_1")]
        [InlineData("key_2")]
        [InlineData("is_test_1")]
        public void Key_ShouldHashed(string key)
        {
            // added
            var def = Create();
            def.SetBool(true, key);
            Assert.True(def.HasKey(key));
            
            // ensure not value
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }

        [Fact]
        public void NullKey_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Create().SetString("test", null));
            Assert.Throws<ArgumentNullException>(() => Create().SetInt(0, null));
            Assert.Throws<ArgumentNullException>(() => Create().SetFloat(0, null));
            Assert.Throws<ArgumentNullException>(() => Create().SetDouble(0, null));
            Assert.Throws<ArgumentNullException>(() => Create().SetBool(false, null));
            Assert.Throws<ArgumentNullException>(() => Create().HasKey(null));
            Assert.Throws<ArgumentNullException>(() => Create().Remove(null));
        }
        
        [Fact]
        public void EmptyKey_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => Create().SetString("test", ""));
            Assert.Throws<ArgumentNullException>(() => Create().SetInt(0, ""));
            Assert.Throws<ArgumentNullException>(() => Create().SetFloat(0, ""));
            Assert.Throws<ArgumentNullException>(() => Create().SetDouble(0, ""));
            Assert.Throws<ArgumentNullException>(() => Create().SetBool(false, ""));
            Assert.Throws<ArgumentNullException>(() => Create().HasKey(""));
            Assert.Throws<ArgumentNullException>(() => Create().Remove(""));
        }
        
        [Fact]
        public void SetString_Null_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => Create().SetString(null, "K-str-null"));
        }

        [Theory]
        [InlineData("k_str_empty", "")]
        [InlineData("k_str_en", "Sample english text")]
        [InlineData("k_str_vn", "Việt Nam")]
        [InlineData("k_str_kr", "농업생산성의 제고와 농지의")]
        [InlineData("k_str_de", "Deutsches Ipsum Dolor meliore Handschuh et Gemeinsamkeit Te Glühwein utamur Sprechen Sie deutsch Exerci Döner eu Lebensabschnittsgefährte Principes Lebensabschnittsgefährte eos Aufschnitt His Projektplanung moderatius Entschuldigung at Bier omnis Entschuldigung epicurei, Umsatzanalyse feugait bitte ei. Mertesacker purto Meerschweinchen te")]
        public void SetString_Pass(string key, string value)
        {
            // set
            var def = Create();
            def.SetString(value, key);
            
            // ensure equal
            Assert.Equal(value, def.StringForKey(key));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }

        [Theory]
        [InlineData("t_value_1", true)]
        [InlineData("t_value_1", false)]
        public void SetBool_Pass(string key, bool value)
        {
            // set
            var def = Create();
            def.SetBool(value, key);
            
            // ensure equal
            Assert.Equal(value, def.BoolForKey(key));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }
        
        [Theory]
        [InlineData("i_value_1", int.MinValue)]
        [InlineData("i_value_1", -1)]
        [InlineData("i_value_1", 0)]
        [InlineData("i_value_1", 1)]
        [InlineData("i_value_1", int.MaxValue)]
        public void SetInt_Pass(string key, int value)
        {
            // set
            var def = Create();
            def.SetInt(value, key);
            
            // ensure equal
            Assert.Equal(value, def.IntForKey(key));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }
        
        [Theory]
        [InlineData("i_value_1", float.MinValue)]
        [InlineData("i_value_1", -1f)]
        [InlineData("i_value_1", -1.32232f)]
        [InlineData("i_value_1", 0f)]
        [InlineData("i_value_1", 1.231f)]
        [InlineData("i_value_1", float.MaxValue)]
        public void SetFloat_Pass(string key, float value)
        {
            // set
            var def = Create();
            def.SetFloat(value, key);
            
            // ensure equal
            Assert.Equal(value, def.FloatForKey(key));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }
        
        [Theory]
        [InlineData("i_value_1", double.MinValue)]
        [InlineData("i_value_1", -1)]
        [InlineData("i_value_1", -1.32232)]
        [InlineData("i_value_1", 0)]
        [InlineData("i_value_1", 1.231)]
        [InlineData("i_value_1", double.MaxValue)]
        public void SetDouble_Pass(string key, double value)
        {
            // set
            var def = Create();
            def.SetDouble(value, key);
            
            // ensure equal
            Assert.Equal(value, def.DoubleForKey(key));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }

        [Fact]
        public void Remove_Pass()
        {
            string key = "test_remove";
            var def = Create();
            def.SetInt(10, key);
            Assert.True(def.HasKey(key));
            
            // remove
            def.Remove(key);
            Assert.False(def.HasKey(key));
        }
        
        [Fact]
        public void Overwrite_Pass()
        {
            string key = "test_overwrite";
            var def = Create();
            
            // set init
            def.SetInt(1, key);
            Assert.Equal(1, def.IntForKey(key));
            
            // overwrite by string
            def.SetString("test_remove", key);
            Assert.Equal("test_remove", def.StringForKey(key));
            
            // overwrite by bool
            def.SetBool(true, key);
            Assert.True(def.BoolForKey(key));
        }

        [Fact]
        public void WrongType_Fail()
        {
            string key = "test_wrongtype";
            var def = Create();
            def.SetInt(1, key);
            
            // get float
            Assert.NotEqual(1, def.FloatForKey(key));
            Assert.Throws<ArgumentOutOfRangeException>(() => Assert.NotEqual(1, def.DoubleForKey(key)));
            Assert.True(def.BoolForKey(key));
            Assert.NotEqual("1", def.StringForKey(key));
        }
    }
}