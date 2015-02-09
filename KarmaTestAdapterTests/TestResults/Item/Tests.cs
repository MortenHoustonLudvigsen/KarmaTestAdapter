using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.Item
{
    public partial class TestResults
    {
        public const string KarmaXml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
            <Karma>
              <Config>
                <LOG_DISABLE>OFF</LOG_DISABLE>
                <LOG_ERROR>ERROR</LOG_ERROR>
                <LOG_WARN>WARN</LOG_WARN>
                <LOG_INFO>INFO</LOG_INFO>
                <LOG_DEBUG>DEBUG</LOG_DEBUG>
                <set/>
                <frameworks>
                  <item>jasmine</item>
                </frameworks>
                <port>53983</port>
                <hostname>localhost</hostname>
                <basePath>C:/Git/KarmaTestAdapter/karma-vs-reporter/test</basePath>
                <files>
                  <item>
                    <pattern>C:\Git\KarmaTestAdapter\karma-vs-reporter\test\node_modules\karma-jasmine\lib/jasmine.js</pattern>
                    <included>true</included>
                    <served>true</served>
                    <watched>false</watched>
                  </item>
                  <item>
                    <pattern>C:\Git\KarmaTestAdapter\karma-vs-reporter\test\node_modules\karma-jasmine\lib/adapter.js</pattern>
                    <included>true</included>
                    <served>true</served>
                    <watched>false</watched>
                  </item>
                  <item>
                    <pattern>C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/*.js</pattern>
                    <served>true</served>
                    <included>true</included>
                    <watched>true</watched>
                  </item>
                  <item>
                    <pattern>C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/*.ts</pattern>
                    <served>false</served>
                    <included>false</included>
                    <watched>true</watched>
                  </item>
                </files>
                <exclude>
                  <item>C:/Git/KarmaTestAdapter/karma-vs-reporter/test/karma.test.conf.js</item>
                  <item>C:/Git/KarmaTestAdapter/karma-vs-reporter/test/karma.test.conf.js</item>
                </exclude>
                <logLevel>INFO</logLevel>
                <colors>false</colors>
                <autoWatch>false</autoWatch>
                <autoWatchBatchDelay>250</autoWatchBatchDelay>
                <usePolling>false</usePolling>
                <reporters>
                  <item>vs</item>
                </reporters>
                <singleRun>true</singleRun>
                <browsers>
                  <item>PhantomJS</item>
                </browsers>
                <captureTimeout>60000</captureTimeout>
                <proxies/>
                <proxyValidateSSL>true</proxyValidateSSL>
                <preprocessors>
                  <item name=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/**/*.js"">
                    <item>vs</item>
                  </item>
                </preprocessors>
                <urlRoot>/</urlRoot>
                <reportSlowerThan>0</reportSlowerThan>
                <loggers>
                  <item>
                    <type>console</type>
                    <layout>
                      <type>pattern</type>
                      <pattern>%p [%c]: %m</pattern>
                    </layout>
                    <makers>
                      <console/>
                    </makers>
                  </item>
                </loggers>
                <transports>
                  <item>websocket</item>
                  <item>flashsocket</item>
                  <item>xhr-polling</item>
                  <item>jsonp-polling</item>
                </transports>
                <plugins>
                  <item>karma-*</item>
                </plugins>
                <client>
                  <args/>
                  <useIframe>true</useIframe>
                  <captureConsole>true</captureConsole>
                </client>
                <browserDisconnectTimeout>2000</browserDisconnectTimeout>
                <browserDisconnectTolerance>0</browserDisconnectTolerance>
                <browserNoActivityTimeout>10000</browserNoActivityTimeout>
                <configFile>C:\Git\KarmaTestAdapter\karma-vs-reporter\test\karma.test.conf.js</configFile>
              </Config>
              <Files>
                <File Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/node_modules/karma-jasmine/lib/jasmine.js"" Served=""true"" Included=""true""/>
                <File Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/node_modules/karma-jasmine/lib/adapter.js"" Served=""true"" Included=""true""/>
                <File Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.js"" Served=""true"" Included=""true"">
                  <Suite Name=""Simple tests 2"" Framework=""jasmine"" Line=""1"" Column=""1"">
                    <Source Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.ts"" Line=""1"" Column=""0""/>
                    <Suite Name=""Nested tests"" Framework=""jasmine"" Line=""8"" Column=""4"">
                      <Source Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.ts"" Line=""8"" Column=""4""/>
                      <Test Name=""should be, that 3 + 12 = 23"" Framework=""jasmine"" Line=""9"" Column=""8"" Index=""276"">
                        <Source Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.ts"" Line=""9"" Column=""8""/>
                      </Test>
                    </Suite>
                  </Suite>
                </File>
                <File Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/Testfile1.js"" Served=""true"" Included=""true"">
                  <Suite Name=""Simple tests"" Framework=""jasmine"" Line=""1"" Column=""1"">
                    <Source Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/Testfile1.ts"" Line=""1"" Column=""0""/>
                    <Test Name=""should be, that 1 + 1 = 2"" Framework=""jasmine"" Line=""2"" Column=""4"" Index=""45"">
                      <Source Path=""C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/Testfile1.ts"" Line=""2"" Column=""4""/>
                    </Test>
                  </Suite>
                </File>
              </Files>
              <Results start=""2015-02-09T08:47:37.234Z"" end=""2015-02-09T08:47:44.212Z"">
                <Browser Name=""PhantomJS 1.9.8 (Windows 8)"">
                  <SuiteResult Name=""Simple tests 2"">
                    <SuiteResult Name=""Nested tests"">
                      <TestResult Name=""should be, that 3 + 12 = 23"" Id=""2"" Time=""5"" Outcome=""Failed"">
                        <Log>Expected 15 to be 23.</Log>
                      </TestResult>
                    </SuiteResult>
                  </SuiteResult>
                  <SuiteResult Name=""Simple tests"">
                    <TestResult Name=""should be, that 1 + 1 = 2"" Id=""4"" Time=""0"" Outcome=""Success""/>
                  </SuiteResult>
                </Browser>
              </Results>
            </Karma>
        ";

        public abstract class ItemTestsHelper : Helper<ItemTestsHelper.ConcreteItem>
        {
            public virtual KarmaTestAdapter.TestResults.Item GetParent()
            {
                return null;
            }

            public virtual XElement GetElement()
            {
                return null;
            }

            public override ConcreteItem CreateItem()
            {
                return new ConcreteItem(GetParent(), GetElement());
            }

            public class ConcreteItem : KarmaTestAdapter.TestResults.Item
            {
                public ConcreteItem(KarmaTestAdapter.TestResults.Item parent, XElement element)
                    : base(parent, element)
                {
                }

                public new KarmaTestAdapter.TestResults.Item Create(XElement element)
                {
                    return base.Create(element);
                }

                public new T GetParent<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetParent<T>();
                }

                public new T GetChild<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetChild<T>();
                }

                public new IEnumerable<T> GetChildren<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetChildren<T>();
                }

                public new IEnumerable<T> GetAllChildren<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetAllChildren<T>();
                }

                public new string Attribute(string name)
                {
                    return base.Attribute(name);
                }

                public new string ValueOfElement(string name = null)
                {
                    return base.ValueOfElement(name);
                }

                public new IEnumerable<XElement> Elements()
                {
                    return base.Elements();
                }

                public new IEnumerable<XElement> Elements(XName name)
                {
                    return base.Elements(name);
                }
            }
        }

        public partial class Item : ItemTestsHelper.Tests<Item.Helper>
        {
            public class Helper : ItemTestsHelper
            {
            }

            public partial class Empty : ItemTestsHelper.Tests<Helper>
            {
            }

            public partial class WithParents : ItemTestsHelper.Tests<WithParents.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    public class Parent1 : ConcreteItem
                    {
                        public Parent1() : base(new Karma((XElement)null), null) { }
                    }

                    public class Parent2 : ConcreteItem
                    {
                        public Parent2() : base(new Parent1(), null) { }
                    }

                    public class Parent3 : ConcreteItem
                    {
                        public Parent3() : base(new Parent2(), null) { }
                    }

                    public override KarmaTestAdapter.TestResults.Item GetParent()
                    {
                        return new Parent3();
                    }
                }
            }

            public partial class WithoutParents : ItemTestsHelper.Tests<Helper>
            {
            }

            public partial class WithElement : ItemTestsHelper.Tests<WithElement.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    private XElement _element = new XElement("Karma",
                        new XAttribute("Path", "The path"),
                        new XElement("Slam", "Bam")
                    );

                    public override XElement GetElement()
                    {
                        return _element;
                    }
                }
            }

            public partial class WithChildren : ItemTestsHelper.Tests<WithChildren.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    public override XElement GetElement()
                    {
                        return XDocument.Parse(KarmaXml).Root;
                    }
                }
            }
        }
    }
}