window.BENCHMARK_DATA = {
  "Content": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          454.9328867594401,
          480.2029307229178,
          453.2711669286092,
          467.0879783630371,
          455.24834285463606,
          450.6970642530001,
          484.41648652003363,
          236.81234884262085,
          235.5320383952214,
          508.80319900512694,
          242.27026520456587,
          500.76539478302004,
          527.4150661740985,
          490.8544050852458,
          482.59102834974016,
          242.05985294977825,
          334.2333171184246,
          421.8871417045593,
          464.33666948171765,
          506.02792006272534,
          477.592585343581,
          231.86652650151933,
          236.16060206095378,
          451.8051270338205,
          240.76155887331282,
          454.22234363555907,
          473.6510327202933,
          369.3130831082662,
          228.74589053789774,
          435.1065190179007,
          454.5234998975481,
          346.25337082999096,
          243.70535846856924,
          442.5956117085048,
          458.5815397671291,
          236.82004312106542,
          354.32197618484497,
          479.5638365064348,
          247.8905804838453,
          334.7561794916789,
          352.3877403259277,
          461.8383591969808,
          540.0930511768048,
          504.466541494642,
          393.13746782938637,
          337.5139049212138,
          360.17263879776,
          432.0374268123082,
          482.2945795059204,
          228.4073472738266
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640,
          640
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          1004.6667348316738,
          1076.5203336988177,
          970.0906133651733,
          1072.214764050075,
          975.5429316929409,
          965.2852711995442,
          957.9963446397048,
          522.3139714558919,
          520.7589690868671,
          966.970623143514,
          557.7056749979655,
          1086.2820332390922,
          1043.5138197580973,
          1017.8715096987211,
          1078.6155003767747,
          536.5991641362508,
          858.4158791028536,
          1114.3946198683518,
          1115.1153530707727,
          967.7987333933512,
          1120.3237092154366,
          560.943596204122,
          539.5012401853289,
          950.5706753049578,
          526.9679295221964,
          995.5603698730469,
          974.1035179773967,
          810.129448890686,
          549.5627836227417,
          1024.498185602824,
          962.0600023905437,
          817.7492109298706,
          574.5917391459147,
          1128.0433668409075,
          963.9214069366456,
          520.9855494181315,
          859.8134956359863,
          1103.4839130129133,
          575.1248204367502,
          751.5023863474528,
          823.6583151817322,
          1049.0578703199114,
          990.3336378733317,
          977.3774132047381,
          896.2365102767944,
          894.5700384286733,
          948.0402340522179,
          923.0064988454183,
          960.2919431413923,
          500.5204112370809
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288,
          3288
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "SuccessStatusCode": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          247.38296123913355,
          246.07338156018938,
          273.4376969973246,
          253.02777807529156,
          286.99714143459613,
          242.2290863257188,
          246.90775027641882,
          134.89341800029462,
          133.47965195973714,
          251.1881541887919,
          136.06530810991924,
          306.5293654123942,
          243.46431465148925,
          256.7388755253383,
          254.96218070983886,
          126.99569839697618,
          217.62261526925224,
          263.3153391224997,
          236.40991017023723,
          236.99189560229962,
          251.82484086354575,
          134.53006947835286,
          132.1548570735114,
          242.41803921972001,
          128.12400733507596,
          233.47819695472717,
          238.17072636286417,
          182.3486209233602,
          132.5479962642376,
          244.69765538435715,
          239.1405565738678,
          174.51684842790877,
          138.9833895609929,
          265.27181117875233,
          242.79161001841229,
          133.49193998972575,
          180.37146946589152,
          251.6106086730957,
          124.58865673201424,
          175.80076657808743,
          205.8272101720174,
          240.5412131037031,
          318.7833679199219,
          252.78348580996195,
          215.81124877929688,
          192.69957841237385,
          229.79083015124004,
          252.84952510197957,
          256.4077988011496,
          120.46900096734365
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528,
          528
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          266.0521447317941,
          275.4768298222468,
          258.9641146977743,
          273.80066193853105,
          247.24771033800565,
          250.62821022669473,
          245.0877154350281,
          143.08223581314087,
          131.42994288035803,
          247.92575775782268,
          139.37534960678644,
          305.0619183608464,
          262.91809272766113,
          260.98547519048054,
          274.15821339289346,
          137.57403254508972,
          244.72689665280856,
          312.8294974840604,
          253.14134384791058,
          249.72960913976033,
          278.4219954490662,
          146.39669446945192,
          147.3075382868449,
          251.80255066553752,
          133.4709383646647,
          242.95159159387862,
          257.9887052853902,
          211.66320983001165,
          146.80514289651597,
          279.4395910331181,
          251.6416536649068,
          210.8067468802134,
          147.95086481021002,
          318.63816400936673,
          240.62100273768107,
          134.57967740297318,
          213.57694873809814,
          285.1488622347514,
          138.163690163539,
          203.67925635973612,
          225.79763158730097,
          261.8787691752116,
          258.11550029118854,
          265.66814473697116,
          238.455833366939,
          247.1441742011479,
          268.7767217499869,
          240.36885870420016,
          237.52215334085318,
          130.85188695362635
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952,
          952
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "Header": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          883.3648537855881,
          1104.9444580811721,
          982.8758127848307,
          979.0195956596962,
          1025.7843687693278,
          988.9927466710409,
          1002.2428723848783,
          530.9531036785671,
          516.7755776087444,
          1064.9942759195962,
          557.0758202626155,
          993.8616753986904,
          1031.6353096008302,
          976.9161876678467,
          1008.7345860345023,
          547.5007173538208,
          734.8453954549936,
          946.3711932046073,
          985.2903900146484,
          975.305111249288,
          1025.0751603444417,
          516.8848755909846,
          527.9362238566081,
          945.5698028564453,
          526.7723495619638,
          919.9278140068054,
          998.9280269622802,
          735.0279777233417,
          544.0216925938925,
          915.0510061264038,
          1014.7478587286813,
          709.499430179596,
          561.6787514005389,
          973.2031075404241,
          970.8140046255929,
          488.33608940669467,
          760.0682645944448,
          997.006590906779,
          543.139175755637,
          691.1171099344889,
          784.1663217544556,
          1069.851177068857,
          986.4647436141968,
          1031.8153250558037,
          856.8820991516113,
          703.2574746268136,
          752.585807459695,
          936.6029814992633,
          1037.7018887656075,
          489.7260799407959
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192,
          1192
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          982.2797829945882,
          1017.240454228719,
          947.1246480941772,
          1027.860305932852,
          937.1546414693197,
          940.3582364400228,
          960.0485731271597,
          501.6144206523895,
          489.83077239990234,
          975.5364428202312,
          537.0733523002037,
          1025.217236328125,
          982.7938363393148,
          1087.29874903361,
          1038.1699677785239,
          526.7911093051617,
          847.2734375635783,
          1086.1869752248128,
          961.5089799245198,
          923.786976369222,
          1047.0305853525797,
          527.0721464157104,
          510.41341979163036,
          988.109370803833,
          509.23218509129117,
          981.7609280904134,
          983.0672017506191,
          779.2889129638672,
          533.6910495758057,
          995.6242108662923,
          961.496369770595,
          778.1785479863485,
          559.0498117719378,
          1092.550279344831,
          933.1230291366577,
          460.394495900472,
          875.9253684557401,
          1076.0143171037946,
          503.38019231160484,
          743.5535878401536,
          832.8873869578043,
          1014.8577059427897,
          1085.575074895223,
          1274.8910056522914,
          876.3387022018433,
          859.7595116297404,
          895.7199317296346,
          937.614222017924,
          1015.780132039388,
          502.50946178436277
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088,
          3088
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "ProblemDetails": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1914.5855023066204,
          1931.6328745524088,
          1870.5461195537023,
          2007.5199420635518,
          1883.6405279795329,
          1884.1201801300049,
          1862.062565939767,
          1152.1087465922037,
          1112.6066719055175,
          1936.7327379079966,
          1167.044155707726,
          1971.1112192789712,
          2001.2130208333333,
          2024.3861897786458,
          1913.9054886744573,
          1137.0291545574482,
          1214.68724661607,
          1649.7600509936992,
          1902.9398686545235,
          1959.3320625850133,
          1983.7660639444987,
          1154.0959295126108,
          1126.5116746266683,
          1856.4731151874248,
          1152.1220690863472,
          1930.158317820231,
          1879.0770250956218,
          1500.763552393232,
          1161.6051745096843,
          1639.1612518310546,
          1878.6429068247478,
          1480.159550857544,
          1174.3412136077882,
          1714.3901007516045,
          1901.673245021275,
          1074.4428888956706,
          1510.8230589548746,
          1980.7205449422202,
          1109.7527424948555,
          1473.8766563415527,
          1420.9827059427896,
          1951.1894088745116,
          1947.2651039123534,
          1970.9589227529673,
          1485.7137116023473,
          1253.4065192086357,
          1333.9417401631674,
          1895.1236245291573,
          1906.353077521691,
          1050.2471432319055
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592,
          1592
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          18786.086873372395,
          22259.949120154746,
          20333.036416860727,
          21958.438153948104,
          21715.138553873698,
          20882.821903773718,
          21538.4646484375,
          10203.588655744281,
          10403.477037702289,
          21332.965475899833,
          11133.035115168645,
          22793.638942173548,
          22024.219218662805,
          22478.297462972005,
          22487.15575467623,
          10507.047151184082,
          13083.991498311361,
          16852.369388834635,
          22497.319272867837,
          20912.696763258715,
          21855.42422281901,
          10704.838727510893,
          10123.619877406529,
          21039.30048014323,
          11069.175037384033,
          21112.943025716148,
          21287.79279581706,
          14938.777777608235,
          10622.285263061523,
          15796.528108869281,
          21027.903159005302,
          14207.212479654949,
          10807.518943277995,
          17299.828063964844,
          21367.002838134766,
          9987.94697679792,
          15325.140707833427,
          20317.872913067156,
          10148.716247558594,
          14251.385841369629,
          13173.81958242563,
          21770.636600748698,
          22032.23585158128,
          22250.64864908854,
          14082.458379109701,
          13245.913962809245,
          14309.641669718425,
          20840.049255371094,
          21596.07994181315,
          9553.765062604632
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34712,
          34712,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34712,
          34713,
          34713,
          34713,
          34712,
          34713,
          34713,
          34713,
          34712,
          34713,
          34713,
          34712,
          34713,
          34713,
          34713,
          34712,
          34712,
          34712,
          34713,
          34713,
          34713
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "QueryParameter": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          719.9084849675496,
          869.9114283879597,
          758.7382371266683,
          907.2899378263033,
          770.6974012374878,
          821.0556493486677,
          806.1221341405596,
          421.73951762517294,
          412.9570006370544,
          798.3946457544963,
          420.7755738894145,
          839.4234569256122,
          835.9516654014587,
          758.2569639205933,
          829.2920172764705,
          411.2909147262573,
          560.4746482372284,
          732.8231026331583,
          833.7528851191203,
          853.7313683373587,
          821.9027952466693,
          426.9092006342752,
          391.9393708889301,
          785.9318697611491,
          423.3627372154823,
          836.2280292510986,
          752.8134021077838,
          573.0018540700277,
          399.9726088523865,
          725.4629685084025,
          770.298097201756,
          568.1776371002197,
          434.57610838753834,
          757.3146889550345,
          748.9378308614095,
          402.4188807805379,
          584.5255438940866,
          769.8451211111886,
          416.11402636307935,
          564.4042382921491,
          604.791879926409,
          759.6970792770386,
          778.333588218689,
          844.1925388063703,
          660.1283797484177,
          549.0496471845187,
          576.4322287877401,
          755.9450314839681,
          864.9657942226955,
          380.53352553503856
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040,
          1040
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          1316.4993698120118,
          1355.2906568967378,
          1304.1272168477376,
          1423.5067085538592,
          1329.6421001434326,
          1294.097818510873,
          1312.2129779543195,
          711.880372267503,
          666.3984788485935,
          1327.2343719482421,
          720.5340617986826,
          1405.2916517893473,
          1367.5818361554827,
          1431.4166532244,
          1414.0908218383788,
          699.2265857549814,
          1102.323692175058,
          1474.845011491042,
          1386.600918451945,
          1278.105143913856,
          1370.6048352559408,
          723.4741080829075,
          721.7052824656168,
          1299.3406219482422,
          703.4429443223136,
          1277.0836035410564,
          1318.5842333475748,
          1067.1857963289533,
          761.9303963343302,
          1333.5948487599692,
          1311.5071291242327,
          1010.3324523925781,
          764.0847593943278,
          1476.635341644287,
          1296.0750034877233,
          703.6973687489827,
          1137.2971270243327,
          1536.3257658640543,
          719.7886102994283,
          1003.669601949056,
          1093.665636698405,
          1371.3303101857502,
          1405.6563457489015,
          1412.7450640360514,
          1210.636733373006,
          1131.2529127756754,
          1149.3772983551025,
          1267.1902982271636,
          1274.663519668579,
          678.7558785756429
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968,
          3968
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "RequestMessage": {
    "commits": [
      {
        "sha": "7558fdb1276a56ca5c9bd06c84299217c9cae701",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:51:58 2026 \u002B0100",
        "message": "feat!: remove the type-only overload of \u0060IgnoringMembers\u0060 in the equivalency options (#2134)"
      },
      {
        "sha": "554746c671599207ec99839f6537076f32e3c135",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:00:46 2026 \u002B0100",
        "message": "fix!: reject an invalid expected JSON of \u0060IsJsonSerializedAs\u0060 when the expectation is written (#2136)"
      },
      {
        "sha": "fa41546d5f555395fb8b6a5361490e5b1c4fe808",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:08:19 2026 \u002B0100",
        "message": "refactor!: put TResult first in IEnumPropertyResult (#2135)"
      },
      {
        "sha": "5fafaf2a3a95a270133c3439dc7ceae77b23bb19",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:16 2026 \u002B0100",
        "message": "fix!: fail the reflection collection expectations and their negations for a null item (#2132)"
      },
      {
        "sha": "2a51fd6f4818e7ddd137aeece75d8a1e34e2ddde",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:38:54 2026 \u002B0100",
        "message": "refactor!: seal the built-in content processors of aweXpect.Web (#2137)"
      },
      {
        "sha": "348ea07491981724d63920d23fa47c9b98ce7a48",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:41:36 2026 \u002B0100",
        "message": "refactor!: hand non-nullable values to \u0060WhoseValues\u0060 of headers and query parameters (#2133)"
      },
      {
        "sha": "91e67fd2855828f597bc08238244c41aeb07c388",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:45:30 2026 \u002B0100",
        "message": "refactor!: move \u0060DiffEngineReporter\u0060 into the namespace \u0060aweXpect.Snapshots.DiffEngine\u0060 (#2140)"
      },
      {
        "sha": "d75e07696149b914c7f4fff5e3d55c26de94d9f4",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 14:46:12 2026 \u002B0100",
        "message": "fix!: map an IPv4-mapped address to IPv4 before the loopback check of \u0060IPAddress\u0060 (#2141)"
      },
      {
        "sha": "8ebddc035231ebc79e6a381b3ef21f2b8c026fef",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:29:39 2026 \u002B0100",
        "message": "fix!: treat a missing content of a request like that of a response (#2138)"
      },
      {
        "sha": "19fa288ec07fe2cb8fef95836967194f4f7e035a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:34:13 2026 \u002B0100",
        "message": "refactor!: align the member-type and parameter filter names of aweXpect.Reflection (#2144)"
      },
      {
        "sha": "dbfb8e56532b271c7886bf19e08dc3c7deeee41e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 15:56:08 2026 \u002B0100",
        "message": "chore: fail the Release build on missing XML documentation (#2143)"
      },
      {
        "sha": "155bfd1e378dc70271e9552005be40d26e8c3e9e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 16:37:45 2026 \u002B0100",
        "message": "fix!: keep encoded slashes distinguishable in \u0060HasPath\u0060 (#2142)"
      },
      {
        "sha": "2bba8222cec064299f4137dceafe29eae2e3ab29",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 17:11:20 2026 \u002B0100",
        "message": "fix: remove ConfigureAwait(false) from the Core evaluation (#2145)"
      },
      {
        "sha": "33ef507b49bfef0fbb997cf69352a8b5776a332d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:27:38 2026 \u002B0100",
        "message": "feat: add \u0060OptionsProvider.WithOptions\u0060 to apply options to a result with or without copy-on-option (#2147)"
      },
      {
        "sha": "f8435557d17b864c209c3b4bdb6124877e64978b",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 20:40:29 2026 \u002B0100",
        "message": "feat!: replace ITimeSystem with TimeProvider (#2148)"
      },
      {
        "sha": "b48cdddbce07676ff9f45bb860f18fd015b90698",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:52:41 2026 \u002B0100",
        "message": "refactor!: build the plural type expectations of aweXpect.Reflection on the generated collection constraints (#2146)"
      },
      {
        "sha": "f256c5caa19b57f3eae9cf26957c86ea90cf5f2e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 21:59:59 2026 \u002B0100",
        "message": "refactor: explain the empty result texts of the collection member constraint (#2150)"
      },
      {
        "sha": "fea532987f5ff65c36e36a5022ccc68563759fa4",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:14:27 2026 \u002B0100",
        "message": "perf: return an already completed result of \u0060SynchronousBoundary.Wait\u0060 without a task (#2149)"
      },
      {
        "sha": "583e9cec3158e6700878a4fe4ba50dcd8ab638d5",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 29 09:41:02 2026 \u002B0100",
        "message": "refactor!: build the remaining plural expectations of aweXpect.Reflection on the generated collection constraints (#2151)"
      },
      {
        "sha": "29daaf7aafd7bdce5a683f50b2c6e14669a78c89",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 07:41:12 2026 \u002B0100",
        "message": "docs: list aweXpect.Chronology with the extension packages (#2152)"
      },
      {
        "sha": "e6330becd917b1e2379c529c3de943ffe46796fc",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 08:23:04 2026 \u002B0100",
        "message": "docs: point out that the documentation describes the v4 pre-release (#2153)"
      },
      {
        "sha": "30129fc05ff99cdceff0c8d8640023c5b8b98d03",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 10:48:36 2026 \u002B0100",
        "message": "docs: separate the extension READMEs from their docs pages (#2154)"
      },
      {
        "sha": "25a97e74e525033a2c2b78cf7501bf25c6690f86",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 11:02:56 2026 \u002B0100",
        "message": "ci: rebuild the documentation site after a release (#2155)"
      },
      {
        "sha": "6c7561407c52a32c2c9bac99a75b40c7b7d3f20a",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 13:00:06 2026 \u002B0100",
        "message": "docs: apply the docs style guide (#2156)"
      },
      {
        "sha": "99f75dcfd82c91f6c9137f0151973fdcb4d1ba11",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 16:28:45 2026 \u002B0100",
        "message": "docs: name the \u0022How it works\u0022 section like its URL (#2157)"
      },
      {
        "sha": "77e01f4be5fa0241c893a7dc47a134573bf43e61",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 31 17:47:24 2026 \u002B0100",
        "message": "docs: add a troubleshooting page (#2158)"
      },
      {
        "sha": "68753ad619945bec43232cb9c94b7061e9ce6ee6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:24:05 2026 \u002B0100",
        "message": "fix: keep the failure when a result context throws (#2159)"
      },
      {
        "sha": "7c05d4594643db36d59ef971c99739a42fc2b43d",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 12:46:14 2026 \u002B0100",
        "message": "fix: compare the host and scheme of a URI ignoring case by default (#2160)"
      },
      {
        "sha": "3f5cc07dfe6a9745cad35fa2eb238f3f9aa2c576",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:14:54 2026 \u002B0100",
        "message": "fix: keep the inner grammar of the Web \u0022which\u0022 wrappers singular and reject blank header names (#2161)"
      },
      {
        "sha": "3f6a8fdf8908f9959d2a45373d35967a032e7f12",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 13:15:30 2026 \u002B0100",
        "message": "docs: lead the v3 migration with Native AOT and complete the v4 changelog (#2162)"
      },
      {
        "sha": "9c72fda89e2f7b4c6f6eb5101a9d4656d6a6d2c6",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 17:23:32 2026 \u002B0100",
        "message": "test: expect the case-insensitive host comparison in the Web which-wrapper grammar test (#2164)"
      },
      {
        "sha": "c2bac0d47570e2e32ffc4a6502f42f5fe45233cc",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Nov 1 18:03:49 2026 \u002B0100",
        "message": "chore: release aweXpect.Xml and aweXpect.Snapshots as stable packages (#2163)"
      },
      {
        "sha": "b53d74ccbbc8a5bbb9259106ff4da558ae15b291",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 07:05:03 2026 \u002B0100",
        "message": "fix: decide a cancellation when it arrives, not when a continuation notices it (#2165)"
      },
      {
        "sha": "3903c85782f1141bf1f5fffca0ae02995a2a2b34",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Nov 2 17:07:04 2026 \u002B0100",
        "message": "test: run the timing tests on the MockTimeSystem of Testably.Abstractions.Testing (#2166)"
      },
      {
        "sha": "c1f4f866a4caaeb3c8248507bc2840e1980b2f72",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:52:45 2026 \u002B0100",
        "message": "chore: Bump the mstest group with 2 updates (#2167)"
      },
      {
        "sha": "36493d03c95f2319d3b2176f9629d208b104d803",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:55:47 2026 \u002B0100",
        "message": "chore: Bump the tunit group with 5 updates (#2168)"
      },
      {
        "sha": "a5ae13ebee799b24ed57c3e0db58c80e291ca73d",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:25 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Testing.Extensions.TrxReport from 2.5.1 to 2.5.2 (#2170)"
      },
      {
        "sha": "5d53b9d11e125c889f7b30d5e81cc7e92d900a4f",
        "author": "dependabot[bot]",
        "date": "Tue Nov 3 00:56:37 2026 \u002B0100",
        "message": "chore: Bump Microsoft.Bcl.TimeProvider from 8.0.0 to 10.0.14 (#2169)"
      },
      {
        "sha": "fa9f5111a92b6d8f9e0cf69f4b5f683406ce9a01",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 01:00:11 2026 \u002B0100",
        "message": "ci: fail the static code analysis job when the Sonar quality gate fails on pull requests (#2171)"
      },
      {
        "sha": "5e27d503d534ca9bdfdd19924d115be92cd12d88",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 08:00:18 2026 \u002B0100",
        "message": "fix: register the cancellation of a delay before its timer (#2172)"
      },
      {
        "sha": "2cb4b2746c27056bc52fe087406754125e645ecf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Nov 3 09:47:24 2026 \u002B0100",
        "message": "test: keep the attached pending snapshot until the test results are written (#2173)"
      },
      {
        "sha": "6f1c949d1fbf518b7fcbebad9145ce675ac539d9",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 06:57:00 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to V8.0.0-pre.2 (#2174)"
      },
      {
        "sha": "7f817d0fa8a90028fe9ada1423b6cde5d9ed387d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 12:48:13 2026 \u002B0100",
        "message": "test: await the cancellation callback in the delay test of the MockTimeSystem (#2175)"
      },
      {
        "sha": "850f7373f0e59301640bab1dcb8c035805ba01d3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 14:41:34 2026 \u002B0100",
        "message": "test: make the tests with real-time limits robust against a loaded machine (#2176)"
      },
      {
        "sha": "0c95f82a0707e6bab18eec3b04ef077ba4b3252c",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Nov 4 21:28:18 2026 \u002B0100",
        "message": "chore: bump Testably.Abstractions.Testing to v8.0.0-pre.3 (#2177)"
      },
      {
        "sha": "13f124fd8545cbd6ae206bb55a9f1ff7126b5137",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 10:12:22 2026 \u002B0100",
        "message": "test: use the strict timer order of the MockTimeSystem where tests depend on it (#2178)"
      },
      {
        "sha": "60f2a5830e95d0205257c1087ef45aede2d5572d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 15:10:20 2026 \u002B0100",
        "message": "test: keep the abandoned task of the ExecutesIn timeout tests pending (#2179)"
      },
      {
        "sha": "4f093bbd7bff3f3f44299da3ae73e202ad6f0bf9",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 18:26:43 2026 \u002B0100",
        "message": "test: use the strict timer order in all tests where a timeout must take effect before a later timer (#2180)"
      },
      {
        "sha": "a5924ccd1acb97e8011a5d4a1ce1341278379a35",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:29:49 2026 \u002B0100",
        "message": "fix: reject null items in the list-valued settings of Core, Reflection and Web (#2184)"
      },
      {
        "sha": "34bd6e692fe6da1a688cef4da2ed5b79b8a42a17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Nov 5 20:34:26 2026 \u002B0100",
        "message": "feat: keep named snapshots in a directory per test file and method (#2181)"
      }
    ],
    "labels": [
      "7558fdb1",
      "554746c6",
      "fa41546d",
      "5fafaf2a",
      "2a51fd6f",
      "348ea074",
      "91e67fd2",
      "d75e0769",
      "8ebddc03",
      "19fa288e",
      "dbfb8e56",
      "155bfd1e",
      "2bba8222",
      "33ef507b",
      "f8435557",
      "b48cdddb",
      "f256c5ca",
      "fea53298",
      "583e9cec",
      "29daaf7a",
      "e6330bec",
      "30129fc0",
      "25a97e74",
      "6c756140",
      "99f75dcf",
      "77e01f4b",
      "68753ad6",
      "7c05d459",
      "3f5cc07d",
      "3f6a8fdf",
      "9c72fda8",
      "c2bac0d4",
      "b53d74cc",
      "3903c857",
      "c1f4f866",
      "36493d03",
      "a5ae13eb",
      "5d53b9d1",
      "fa9f5111",
      "5e27d503",
      "2cb4b274",
      "6f1c949d",
      "7f817d0f",
      "850f7373",
      "0c95f82a",
      "13f124fd",
      "60f2a583",
      "4f093bbd",
      "a5924ccd",
      "34bd6e69"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1279.2880212238856,
          1346.1458651678902,
          1269.948884646098,
          1378.3229776109968,
          1323.4745032446724,
          1278.3616327139048,
          1329.4111164637975,
          782.1566386222839,
          751.449466569083,
          1376.6437390191215,
          808.4705751859225,
          1375.4480187733968,
          1351.9152256011962,
          1322.2692782538277,
          1397.5895426823542,
          801.633157661983,
          907.5709164301554,
          1133.8865387256328,
          1380.075529607137,
          1360.5626458485922,
          1360.7830748240153,
          832.5791413625082,
          809.5426233291626,
          1326.0578970591228,
          769.7036573546274,
          1277.692675145467,
          1229.8665685653687,
          1037.7501219235933,
          798.6245455375084,
          1102.595392363412,
          1268.6659764607748,
          968.3800178255353,
          805.9701806477138,
          1172.9074870518275,
          1303.7188658033099,
          788.4071518091055,
          1013.9994605137751,
          1277.583984375,
          753.9521387645176,
          971.027081044515,
          941.2873355320522,
          1291.4382766723634,
          1329.0484181722006,
          1398.152526174273,
          1057.3421249389648,
          835.2240226745605,
          928.5770222345988,
          1292.3036208519568,
          1288.1728463172913,
          754.820012664795
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "aweXpect memory",
        "unit": "b",
        "data": [
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440,
          1440
        ],
        "borderColor": "#63A2AC",
        "backgroundColor": "#63A2AC",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "FluentAssertions time",
        "unit": "ns",
        "data": [
          772.4098425547282,
          807.9884814580281,
          731.0361295846792,
          792.1811717351278,
          750.3739574432373,
          705.4851949214935,
          725.3763034684317,
          433.80017140706383,
          432.911708577474,
          764.6549742380778,
          441.66459156916693,
          830.035285504659,
          812.2311939239502,
          788.6927172978719,
          835.8459091186523,
          429.7902944638179,
          730.3245754241943,
          908.6576529184978,
          770.9685313542684,
          802.5459557851156,
          825.9286317825317,
          457.9500850530771,
          406.5107835622934,
          755.7857846577962,
          431.18586828158453,
          771.6296712875367,
          809.2666116714478,
          644.9650718053182,
          431.4190220514933,
          813.2444582621257,
          754.167457989284,
          605.4867894990103,
          473.9023963671464,
          928.4233367919921,
          728.4841364542643,
          437.62008206049603,
          670.3013818105062,
          869.249768447876,
          421.2064799149831,
          589.850031153361,
          662.7730319159372,
          791.8382557551066,
          794.4581047693888,
          785.8670385360717,
          720.573264280955,
          727.2604808807373,
          774.9994736353557,
          714.9428385954636,
          729.838121287028,
          396.62689444224037
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "FluentAssertions memory",
        "unit": "b",
        "data": [
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768,
          2768
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  }
}