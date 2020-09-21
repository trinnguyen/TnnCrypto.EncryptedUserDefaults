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
            Assert.True(def.Contains(key));
            
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
            Assert.Throws<ArgumentNullException>(() => Create().Contains(null));
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
            Assert.Throws<ArgumentNullException>(() => Create().Contains(""));
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
            Assert.Equal(value, def.StringForKey(key, null));
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
            Assert.Equal(value, def.BoolForKey(key, false));
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
            Assert.Equal(value, def.IntForKey(key, 0));
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
            Assert.Equal(value, def.FloatForKey(key, 0));
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
            Assert.Equal(value, def.DoubleForKey(key, 0));
            Assert.False(def.UserDefaults.ToDictionary().ContainsKey(new NSString(key)));
        }

        [Fact]
        public void Remove_Pass()
        {
            string key = "test_remove";
            var def = Create();
            def.SetInt(10, key);
            Assert.True(def.Contains(key));
            
            // remove
            def.Remove(key);
            Assert.False(def.Contains(key));
        }
        
        [Fact]
        public void Overwrite_Pass()
        {
            string key = "test_overwrite";
            var def = Create();
            
            // set init
            def.SetInt(1, key);
            Assert.Equal(1, def.IntForKey(key, 0));
            
            // overwrite by string
            def.SetString("test_remove", key);
            Assert.Equal("test_remove", def.StringForKey(key, null));
            
            // overwrite by bool
            def.SetBool(true, key);
            Assert.True(def.BoolForKey(key, false));
        }

        [Fact]
        public void WrongType_Float_Double_Failed()
        {
            string key = "test_wrongtype";
            var def = Create();
            def.SetInt(1, key);
            
            // get float
            Assert.NotEqual(1, def.FloatForKey(key, 0));
            Assert.NotEqual(1, def.DoubleForKey(key, 0));
            Assert.NotEqual("1", def.StringForKey(key, ""));

            // boolean is true (1 -> true)
            Assert.True(def.BoolForKey(key, false));
        }

        [Fact]
        public void WrongType_String_Double_Failed()
        {
            string key = "test_wrongtype_str_d";
            var def = Create();
            def.SetString("3.45", key);

            // get float
            Assert.NotEqual(3.45, def.DoubleForKey(key, 0));
            Assert.Equal(0, def.DoubleForKey(key, 0));
        }

        [Fact]
        public void GetValue_NotExists_Default()
        {
            var def = Create();
            string key = "key_not_exists";

            // get string
            Assert.Equal("example_def", def.StringForKey(key, "example_def"));
            Assert.True(def.BoolForKey(key, true));
            Assert.False(def.BoolForKey(key, false));
            Assert.Equal(int.MaxValue, def.IntForKey(key, int.MaxValue));
            Assert.Equal(-3.45f, def.FloatForKey(key, -3.45f));
            Assert.Equal(3.144567890d, def.DoubleForKey(key, 3.144567890d));
        }
    }
}