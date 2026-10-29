window.BENCHMARK_DATA = {
  "Content": {
    "commits": [
      {
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          375.13996621540616,
          466.0779568354289,
          465.34659481048584,
          450.48752886454264,
          474.81991254366363,
          333.31606578826904,
          261.96037473678587,
          435.30090980529786,
          445.07985293865204,
          475.11523383458456,
          409.07895781199136,
          229.30270613156833,
          501.08385056715747,
          243.95742290360587,
          468.22961873274585,
          486.36500345865886,
          223.5864475795201,
          462.89923133850095,
          352.1508848508199,
          334.1013118823369,
          446.9169069358281,
          253.70347741671972,
          494.79986374194806,
          343.99022534688316,
          316.39989782969155,
          453.71519371668495,
          330.5044671058655,
          250.1517732302348,
          456.01056003570557,
          212.62073295457023,
          478.6094778060913,
          457.49622230529786,
          488.1308433532715,
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
          334.2333171184246
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
          664,
          664,
          664,
          664,
          664,
          664,
          664,
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
          838.691475613912,
          1043.5027585710798,
          1003.1831247965495,
          944.0112840212308,
          965.7402460915702,
          718.939692565373,
          531.1579947153728,
          1085.7083005905151,
          1050.0738285064697,
          943.4061141380897,
          954.0227118174236,
          535.0919495991299,
          1055.40873840877,
          508.508009161268,
          1082.931537882487,
          929.5945545832316,
          512.6541816847665,
          963.3389231363932,
          807.728419303894,
          752.383396911621,
          974.1474817276,
          514.9583790779113,
          1034.8770593007405,
          910.86525787626,
          787.7659174374172,
          955.0897411982219,
          737.6938452039446,
          507.0532956441244,
          1136.7805593930757,
          474.0762982368469,
          1070.9929101126534,
          1021.4798039027622,
          925.5554426738194,
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
          858.4158791028536
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
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          175.41801625887553,
          261.6131752899715,
          253.12911984125773,
          270.0252052034651,
          255.85546283721925,
          167.3502496310643,
          121.32872117360434,
          257.48342105320523,
          233.35180184046428,
          245.242271232605,
          232.44820243517557,
          137.56965975761415,
          268.5558290799459,
          144.42747661045618,
          279.2687221209208,
          250.16857674916585,
          123.39553442001343,
          273.8161085764567,
          179.8098295688629,
          169.15943902333578,
          264.4845407803853,
          127.60655578545162,
          253.66847105026244,
          205.55006698199682,
          186.2806418623243,
          242.04026679992677,
          180.1076249395098,
          121.71482060636792,
          269.49409039815265,
          113.97554241759437,
          249.41025761195593,
          259.32427620887756,
          231.6196001493014,
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
          217.62261526925224
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
          217.27103929519654,
          279.8982596056802,
          279.40837100346886,
          243.9263629572732,
          280.30038595199585,
          202.16065100034078,
          136.62280816691262,
          304.17708123524983,
          250.91883274487085,
          244.22368756930032,
          257.37823670251026,
          148.23730926513673,
          279.078387705485,
          147.87248187405723,
          250.4324870109558,
          248.4758246285575,
          130.65717989206314,
          249.7723609606425,
          210.57789204915363,
          203.72975311960494,
          240.6578016678492,
          137.60128811427526,
          268.36130701700847,
          252.2637170278109,
          217.91079696019492,
          255.67062606811524,
          202.8274187019893,
          126.63684141635895,
          320.31061731974285,
          122.06529651369367,
          273.7050013175377,
          267.05627039500644,
          247.11147050857545,
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
          244.72689665280856
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
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          522.8357938766479,
          788.2841656366984,
          675.4387560571943,
          708.9466533025105,
          740.9117800394694,
          536.5196068627494,
          382.47118148803713,
          726.9388621648153,
          750.9066850798471,
          746.1851156779697,
          652.6387585957845,
          405.7758544286092,
          831.4142941792805,
          393.1221553166707,
          746.8927319600032,
          736.819604365031,
          374.3287629347581,
          797.5961325963339,
          515.6820469636184,
          517.1180922826131,
          726.4629237835223,
          379.5079736709595,
          742.8550732930502,
          628.7902672449748,
          536.3601025263469,
          728.1997217450823,
          512.3724195480347,
          366.1072335924421,
          744.1217367989676,
          475.842946879069,
          1032.5758972167969,
          976.4981702168783,
          970.3885461262295,
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
          734.8453954549936
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
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
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
          773.9067679087321,
          1053.388213634491,
          932.5688414255778,
          978.5534809657505,
          939.4449716976711,
          690.8980956713359,
          512.1857842717852,
          1062.4057080405098,
          1006.8786333719889,
          924.3652637004852,
          933.8703877585275,
          488.77961564064026,
          1034.8188137054444,
          499.79274008824274,
          991.5358709607806,
          1021.7144416809082,
          472.1112215360006,
          988.8625738961356,
          742.9681043624878,
          746.5553225003756,
          994.1840599060058,
          515.1387907663981,
          1015.0835796356201,
          852.0393388271332,
          715.3524859973362,
          929.0748093922933,
          737.6815742651621,
          469.7671685900007,
          1102.59083543505,
          454.2289246045626,
          1101.2028221130372,
          1027.4066754659018,
          926.7154275349209,
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
          847.2734375635783
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
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1474.9206541606359,
          1950.0860214233398,
          1916.9496900118315,
          1830.7527633666991,
          1853.697654724121,
          1443.517444904034,
          1093.0504610355083,
          1666.9366144452777,
          1956.8127754756383,
          1900.61237894694,
          1547.4663707733155,
          1133.1822125361516,
          1926.0909589131672,
          1177.2920851026263,
          1958.2026219685872,
          1858.7134050641741,
          1058.938818522862,
          1859.921594619751,
          1489.384662882487,
          1458.4528450012208,
          1833.6947093376746,
          1185.7119279588971,
          1907.9598112742106,
          1231.5092565096343,
          1165.2582729046162,
          1827.0886987958636,
          1422.0985904693603,
          1113.2513363701958,
          1676.5335039774577,
          1048.671103922526,
          1958.8783255985804,
          1910.9118482149565,
          1955.3485249110631,
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
          1214.68724661607
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
          14162.28881632487,
          22872.52804347447,
          18003.7796081543,
          20633.74044565054,
          21957.443739827475,
          13618.182097371418,
          9516.283680216471,
          16920.19539097377,
          21707.599420166014,
          20605.44338285006,
          15618.068280537924,
          9900.679490152996,
          21972.503662109375,
          10465.854170663017,
          21861.020222255163,
          22040.96430460612,
          9506.014527384441,
          21547.44328090123,
          13856.134032185872,
          14278.819517008464,
          21090.26869506836,
          9712.709330967495,
          22823.0644144694,
          13360.437669020434,
          12651.129877726236,
          20858.086165364584,
          13633.464186604817,
          10508.656198774066,
          17193.50115356445,
          9380.347384134928,
          22459.962811279296,
          22429.298300606864,
          20191.194490559898,
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
          13083.991498311361
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
          34712,
          34713,
          34713,
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
          34713,
          34712,
          34712,
          34713,
          34713,
          34713,
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
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34713,
          34712
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
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          575.106980641683,
          844.0996187073844,
          712.0765828352708,
          778.9961109161377,
          768.714071053725,
          553.0570297241211,
          382.4145840326945,
          730.5490020751953,
          734.887543741862,
          763.7747718811036,
          690.80318238185,
          410.69397364343916,
          866.7201562608991,
          433.6317463602339,
          769.6820955276489,
          815.0018540791103,
          389.4335874239604,
          834.0550672824567,
          575.5524276097616,
          553.629864011492,
          806.9628481183734,
          405.7249538217272,
          868.1512826283773,
          563.2277349063328,
          524.170546736036,
          746.4120240211487,
          552.8954482396443,
          408.56006152289257,
          750.5178563044622,
          410.72088912328087,
          790.5419354120891,
          798.5347500528608,
          732.9516418163593,
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
          560.4746482372284
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
          1015.6596185139248,
          1433.901644007365,
          1294.1110699517387,
          1291.4563020070393,
          1354.8167089315561,
          966.232188351949,
          660.8005938847859,
          1455.4517759595599,
          1358.3833380381266,
          1278.8678690592449,
          1239.7863914489747,
          680.9078979492188,
          1371.1180374962944,
          737.3961765766144,
          1362.0484779064473,
          1282.5236325945173,
          666.4958557764689,
          1314.3172190348307,
          1041.6467595418294,
          1005.5401215871175,
          1263.6000592367989,
          676.403100673969,
          1431.0748596191406,
          1150.0280779202778,
          1047.33023147583,
          1329.397498675755,
          970.3872172491891,
          678.8819772720337,
          1474.2933091481527,
          688.5845597130912,
          1437.2649509429932,
          1410.389205796378,
          1255.4042546198918,
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
          1102.323692175058
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
        "sha": "7649b140c45108e1ba88a6357920b4c104edc29e",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:44:21 2026 \u002B0100",
        "message": "feat!: support string options in generated collection expectations (#2096)"
      },
      {
        "sha": "f244417826014c0ea772948e7d7f4543c50afdad",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:45:24 2026 \u002B0100",
        "message": "feat!: make AreEqualTo of non-generic items generic over the collection (#2098)"
      },
      {
        "sha": "69ead4c355e918932603cbce8d175808d988620a",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 21:46:23 2026 \u002B0100",
        "message": "fix: accept a nullable string accessor in AreUnique and AreNotUnique (#2099)"
      },
      {
        "sha": "acd69449439d332fcb4717597e4d421649697b67",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:02:10 2026 \u002B0100",
        "message": "feat!: parse a string expected value as JSON text in the Json matching expectations (#2102)"
      },
      {
        "sha": "a2f7290430e0fa5702891a597a3db376229e9e19",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 22:05:15 2026 \u002B0100",
        "message": "feat!: rename ComparedWith to Using in the snapshot results (#2103)"
      },
      {
        "sha": "55068d5fe7d976abc5eea2df84c1a742dea032db",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 26 23:35:44 2026 \u002B0100",
        "message": "refactor!: hold the Json lookup option in an OptionsHolder (#2101)"
      },
      {
        "sha": "de6df36f17ab03e0f9c6dfe3398a1668430e49f0",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 00:15:59 2026 \u002B0100",
        "message": "feat!: hand out non-null Xml nodes and add attribute and value results (#2105)"
      },
      {
        "sha": "fc7e1e8a45717c5e26c4ce9182b86436a02d0a8f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:08:21 2026 \u002B0100",
        "message": "chore: create the options holder of an expectation only when an option is set (#2078)"
      },
      {
        "sha": "031fdf5a95f0ddf531175e2c6bcc42dc64381ae8",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:09:28 2026 \u002B0100",
        "message": "fix: let WithExtension accept the nullable public API snapshot result (#2110)"
      },
      {
        "sha": "67f3083889edf9f648841e1a1483eb9bdf9350b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:10:26 2026 \u002B0100",
        "message": "refactor!: move AccessModifiers to the aweXpect.Reflection namespace (#2111)"
      },
      {
        "sha": "2e0f7001163c641048381150afa3fd5da9a84b9b",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:11:33 2026 \u002B0100",
        "message": "fix: apply the formatters of an expectation to the text of a failed negation (#2112)"
      },
      {
        "sha": "fa3e0676c44578eafcf46fd152ec7bdec5e21e66",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:14:39 2026 \u002B0100",
        "message": "refactor!: accept optional options in PublicApiDeclarations.GenerateFrom (#2114)"
      },
      {
        "sha": "144c7b566ef277dafd988bff3a188151004e9ea8",
        "author": "dependabot[bot]",
        "date": "Tue Oct 27 06:36:15 2026 \u002B0100",
        "message": "chore: Bump System.Text.Json from 10.0.13 to 10.0.14 (#2117)"
      },
      {
        "sha": "b091e8114b027c56b6a80147aaf14592786cfa31",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 06:56:27 2026 \u002B0100",
        "message": "fix: let the text selection of snapshot rules return a copy for each option (#2108)"
      },
      {
        "sha": "84c8c5e0f0d2ec600ea8b72503b6f699b3a78a62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:02:10 2026 \u002B0100",
        "message": "feat: report discarded option calls of the extension packages (#2106)"
      },
      {
        "sha": "e2339ca9a22fc61056cbab192219757f0a0246af",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:03:57 2026 \u002B0100",
        "message": "feat: add the dependency and operator type filters and document OrHas and the parameter options (#2115)"
      },
      {
        "sha": "9ea73f6f957675be0127e9090d75e38b40c85501",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:04:35 2026 \u002B0100",
        "message": "fix: never compile snapshot files of a test project (#2118)"
      },
      {
        "sha": "057732a7cdbc31d3dd8351c15e3d07cbb8215354",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:06:15 2026 \u002B0100",
        "message": "feat!: split the snapshot result by the kind of subject (#2116)"
      },
      {
        "sha": "cda17b83ab02414e56206b4594068efb067cd556",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 08:07:05 2026 \u002B0100",
        "message": "fix: separate the filter descriptions of the In.Type and In.Types sources (#2119)"
      },
      {
        "sha": "3c243dad2ed3b1b2e9c9d06cc260a3e98f2a1d33",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 09:32:53 2026 \u002B0100",
        "message": "test: drop the trailing space from the expected operator filter descriptions (#2120)"
      },
      {
        "sha": "dfdf7bc1c10168eb950b8f7a189da4fc77f183b9",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:26:30 2026 \u002B0100",
        "message": "feat!: align the package options of Json, Xml and PublicApi (#2113)"
      },
      {
        "sha": "31332abd37c441bcbd7aeb12b152f46ffafbbd37",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 10:53:41 2026 \u002B0100",
        "message": "feat!: hand out nodes from the collection views of JsonNode and keep JsonValue in IsNumber\u003CTNumber\u003E (#2107)"
      },
      {
        "sha": "2ba23029a67b913f53b6d8ea1abd535021d43d13",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 11:58:57 2026 \u002B0100",
        "message": "feat!: pass the cancellation token to the filters of aweXpect.Reflection (#2121)"
      },
      {
        "sha": "630e8f0a732515f8914fa11739bb89c446261949",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:14:35 2026 \u002B0100",
        "message": "feat!: list the snapshot and public API options in the failure (#2123)"
      },
      {
        "sha": "76bf298b09d57623575adaa9ce075b6160776c23",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:25:21 2026 \u002B0100",
        "message": "perf: parse the expected JSON once and the subject of IsValidJson only once (#2122)"
      },
      {
        "sha": "26a495951625b114b6cf7b537128b31877629eee",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:43:40 2026 \u002B0100",
        "message": "feat: add nested expectations on the request URI of an HttpRequestMessage (#2124)"
      },
      {
        "sha": "6f944ab7faeab06284ef31c629938e5fa72e711f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 13:46:43 2026 \u002B0100",
        "message": "feat!: split HasProblemDetailsContent by whether a type is expected (#2125)"
      },
      {
        "sha": "7a6e19617e9c8ee5464332377e26992643973135",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 16:46:01 2026 \u002B0100",
        "message": "refactor!: name the expected values of problem details \u0060expected\u0060 (#2126)"
      },
      {
        "sha": "23f4f8ec6343b5206ddc9b5ca44403d07ad31baf",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:24:28 2026 \u002B0100",
        "message": "fix: drop the nested grammar in the Web member expectations ending in \u0022which\u0022 (#2127)"
      },
      {
        "sha": "978086a87d8ddd581d2190898f2709ff9cab5e36",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 27 17:28:44 2026 \u002B0100",
        "message": "feat!: let the header expectations also read the content headers (#2128)"
      },
      {
        "sha": "e410691eac21dd22470dd9b8afbc090e08e0770e",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 12:13:01 2026 \u002B0100",
        "message": "docs: amend the design goals where they are worded too broadly (#2129)"
      },
      {
        "sha": "0027e2384765d953be05b57888e8915cfdcc923d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:13:33 2026 \u002B0100",
        "message": "fix!: fail \u0060WhoseValue\u0060 when a header or query parameter does not have exactly one value (#2131)"
      },
      {
        "sha": "763310401c57e5ccb7d4ec27877203bcdfa8be22",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 28 13:22:27 2026 \u002B0100",
        "message": "fix!: make the Json and Xml option methods available with only \u0060using aweXpect;\u0060 (#2130)"
      },
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
      }
    ],
    "labels": [
      "7649b140",
      "f2444178",
      "69ead4c3",
      "acd69449",
      "a2f72904",
      "55068d5f",
      "de6df36f",
      "fc7e1e8a",
      "031fdf5a",
      "67f30838",
      "2e0f7001",
      "fa3e0676",
      "144c7b56",
      "b091e811",
      "84c8c5e0",
      "e2339ca9",
      "9ea73f6f",
      "057732a7",
      "cda17b83",
      "3c243dad",
      "dfdf7bc1",
      "31332abd",
      "2ba23029",
      "630e8f0a",
      "76bf298b",
      "26a49595",
      "6f944ab7",
      "7a6e1961",
      "23f4f8ec",
      "978086a8",
      "e410691e",
      "0027e238",
      "76331040",
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
      "f256c5ca"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          988.6311305999756,
          1391.9356756210327,
          1265.7013289133708,
          1326.7882439931234,
          1376.466786112104,
          971.250758488973,
          782.1104073842366,
          1134.3332211421086,
          1307.040870507558,
          1281.0242557525635,
          1062.8954622268677,
          767.6678476333618,
          1387.4547334035237,
          807.4877299528855,
          1320.3673434938703,
          1287.7322060902914,
          728.2057986940656,
          1281.492359161377,
          1023.9611812319074,
          988.8656174341837,
          1256.6393885294597,
          791.4559697423663,
          1387.392942564828,
          839.8379922279945,
          784.4232167402903,
          1277.230293146769,
          1020.0592324393136,
          733.1517812093099,
          1151.0647342999775,
          744.3508659998575,
          1373.2333023364727,
          1326.3084732055663,
          1237.9065493070161,
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
          907.5709164301554
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
          1464,
          1464,
          1464,
          1464,
          1464,
          1464,
          1464,
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
          617.5446417672293,
          855.5677131925311,
          811.6032245635986,
          719.9702410380046,
          833.0914911905925,
          589.5085382461548,
          401.15389658610025,
          892.6884525934855,
          760.1046389897664,
          808.0055464426677,
          747.9362820943196,
          450.775159962972,
          797.6576168060303,
          442.80978135267895,
          831.9102221171062,
          786.7034755071004,
          404.8780078206743,
          734.9104886054993,
          647.6953303997333,
          614.9810211181641,
          725.9248662312825,
          411.2725131670634,
          830.2182019551595,
          742.6991150038583,
          654.2161556879679,
          728.3287561416626,
          598.7262877782185,
          390.1776904741923,
          924.860945447286,
          377.19361083848133,
          836.3457184473674,
          862.400163430434,
          740.1350386483329,
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
          730.3245754241943
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