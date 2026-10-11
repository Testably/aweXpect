window.BENCHMARK_DATA = {
  "Equivalency": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          12836.889825674203,
          13222.416271754673,
          10495.002156575521,
          12761.611132108248,
          7007.373010762532,
          12983.394603474935,
          10084.662989298502,
          6555.821635564168,
          12535.305913289389
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
          4032,
          4032,
          4032,
          4032,
          4032,
          4032,
          4032,
          3040,
          3040
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
          374409.37545572914,
          382532.4231770833,
          294336.8800223214,
          309036.89704241074,
          187409.99231770833,
          359474.48984375,
          258528.38291015624,
          189413.85319010416,
          374195.22265625
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
          590287,
          590292,
          590224,
          590292,
          590288,
          590292,
          590287,
          590224,
          590292
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          19419.396723429363,
          20312.683101109094,
          17760.76099068778,
          17931.169439697267,
          10557.885479518345,
          19355.007954915363,
          14237.522770690917,
          12166.199898856026,
          18772.20353581355
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          15680,
          15680,
          15680,
          15680,
          15680,
          15680,
          15680,
          15680,
          15680
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "ItemsCount_AtLeast": {
    "commits": [
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          181.82756209373474,
          306.9443253449031
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
          600,
          600
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
          331.0620517730713,
          504.2283857890538
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
          2008,
          2008
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          261.37859092439925,
          408.87290208680287
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          1064,
          1064
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "Int_GreaterThan": {
    "commits": [
      {
        "sha": "647847aae6bff0b87edd4fb2b0f31ea389537e57",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:31 2026 \u002B0200",
        "message": "fix: enumerate the expected sequence of collection \u0060IsEqualTo\u0060 and \u0060IsContainedIn\u0060 only once per expectation (#1668)"
      },
      {
        "sha": "272bbb23db0b17bf4bfdffd1a5431707dacffa73",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:59 2026 \u002B0200",
        "message": "fix: skip every type that is compared by value or by content in the metadata generator (#1667)"
      },
      {
        "sha": "23b6cd56dbd45d5be7a56d4d2fc91ba0f1b6fba1",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:15:43 2026 \u002B0200",
        "message": "fix: accept finite numbers whose distance overflows in strict ordering with an infinite tolerance (#1666)"
      },
      {
        "sha": "a1b234c6becec264c0e5affca08319172f105a7c",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:16:17 2026 \u002B0200",
        "message": "fix!: do not offer a second \u0060Matching\u0060 on the result of \u0060HasSingle().Matching\u2026\u0060 (#1663)"
      },
      {
        "sha": "168dc965e6b1c84d256ac40eb02d77d52b142043",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 22:13:55 2026 \u002B0200",
        "message": "fix!: reject a property expression of \u0060TriggeredPropertyChangedFor\u0060 that is no property of the subject (#1665)"
      },
      {
        "sha": "f21e6eaba602e779f0efe0ae83e392121e673f0e",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 06:36:57 2026 \u002B0200",
        "message": "refactor: make the timeout and retry tests independent of the machine speed (#1658)"
      },
      {
        "sha": "d98a48a7c9027a5e9a3fb8092b2b7eb259848f11",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 07:20:25 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.16 (#1670)"
      },
      {
        "sha": "0c874997620abfae9aec2928a18f9bffbd2ee840",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 08:34:19 2026 \u002B0200",
        "message": "refactor: reduce the cognitive complexity of \u0060EmitRegistration\u0060 in the \u0060TypeMetadataGenerator\u0060 (#1671)"
      },
      {
        "sha": "0d6d920b6328f9cb9a22fe6e17b1c17be8dd5cfa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 18:34:38 2026 \u002B0200",
        "message": "test: migrate the unit tests from xUnit v2 to TUnit (#1672)"
      },
      {
        "sha": "c8e40f0059cc58502f272fc46bb6c8133187ecb3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:07:05 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.17 (#1687)"
      },
      {
        "sha": "a8e6cbb0921667eb9773fa25913c69b0c4430849",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:17:05 2026 \u002B0200",
        "message": "test: make real-time dependent tests of \u0060Eventually\u0060, \u0060Polling\u0060 and the signaler deterministic (#1686)"
      },
      {
        "sha": "82b90d2ea7e6b0152df0739bbbee5fdaada69a39",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:50:55 2026 \u002B0200",
        "message": "ci: identify the pull request of an analysed run by the event instead of its artifacts (#1688)"
      },
      {
        "sha": "e0c39d6f3fd9d409274b60c5a932bada9f55a9c0",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:40:10 2026 \u002B0200",
        "message": "fix: describe the last attempt in \u0060IsOneOf\u0060 and \u0060IsNotOneOf\u0060 on \u0060DateTime\u0060 after an earlier incompatible kind (#1689)"
      },
      {
        "sha": "3a0d86a45c60afc2bdef37100570c1e1ac13bddc",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:53:09 2026 \u002B0200",
        "message": "ci: disable the mutation tests and pin the Stryker version (#1693)"
      },
      {
        "sha": "e4877b71814748aeb8eae0bfbd3698446d51a28a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:11:36 2026 \u002B0200",
        "message": "test: pin the NUnit 4 adapter tests to NUnit 4 and add a project for NUnit 5 (#1690)"
      },
      {
        "sha": "e4e4a0e6d5366735cd810bd521c7f2cc8e8202c1",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:39:49 2026 \u002B0200",
        "message": "fix!: hand out a non-null subject when awaiting \u0060IsParsableInto\u0060 on a string (#1691)"
      },
      {
        "sha": "9f96f8b98b0347930a55827ba71f5ac3a27432eb",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:42:31 2026 \u002B0200",
        "message": "ci: close gaps in the test runs and the public API check of pull requests (#1692)"
      },
      {
        "sha": "a07a99e21c62fcc44d71c973dc607c32b262670a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 15:25:38 2026 \u002B0200",
        "message": "fix: bound the check of an attempt of \u0060Eventually()\u0060 by the retry budget (#1695)"
      },
      {
        "sha": "998fbcba07f55588650d37e481c620b6925b626d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:09:27 2026 \u002B0200",
        "message": "fix: fail the expectation when the enumerator or the current item of a materialized subject throws (#1697)"
      },
      {
        "sha": "464696d177a9f9c12b445c4c827f1ac2a073c091",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:11:22 2026 \u002B0200",
        "message": "fix: keep the outcome of a quantified expectation when user code throws after the deciding item of a collection with a known count (#1698)"
      },
      {
        "sha": "1f967b5891c9d2474c74e7122df47418ba35c8c0",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 12:19:03 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.18 (#1740)"
      },
      {
        "sha": "7e4f4bdba98cfa1eddad59a18b83cbdf776c174d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:14:00 2026 \u002B0200",
        "message": "fix: show the whole string difference when the minimum number of characters after it is close to \u0060int.MaxValue\u0060 (#1741)"
      },
      {
        "sha": "93d22f80c1c71b9006b4950886cc204fc899957c",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:23:23 2026 \u002B0200",
        "message": "fix: do not suppress nullability warnings when the statement of the expectation overwrites the subject (#1742)"
      },
      {
        "sha": "8b666ac8f8b63072a96fd3152182e03b2cd192f8",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:31:43 2026 \u002B0200",
        "message": "fix!: check the kinds of \u0060DateTime\u0060 values in an untyped collection when verifying its order (#1744)"
      },
      {
        "sha": "8c8f6bf6b6495d38ccf74cdd4c25ae541f0b3e6d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:36:07 2026 \u002B0200",
        "message": "fix: keep an event recording running across the repeated checks of \u0060CompliesWith(\u2026).Within(\u2026)\u0060 (#1745)"
      },
      {
        "sha": "6242c56e2ef28022bdbd22a141f2dd24a3d30374",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:50:50 2026 \u002B0200",
        "message": "fix!: pass a null argument to the predicate of the positional event filters (#1747)"
      },
      {
        "sha": "f7a0873ff8b24ee55e1f59c3990cf1ab57f61be6",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 16:06:38 2026 \u002B0200",
        "message": "fix: stop reading the source for the failure message of the synchronous \u0060IsEmpty\u0060 (#1746)"
      },
      {
        "sha": "8e2da4f371eda53d393d9e80ba7a07a78b9b2905",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:01:52 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.19 (#1753)"
      },
      {
        "sha": "41cb59cca90ed6a485f0c71418670b19988dc75b",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:34:01 2026 \u002B0200",
        "message": "docs: shorten the v3 migration guide and trim the reference pages (#1754)"
      },
      {
        "sha": "75cde8b0fa6fde9400552cb23a296a087933c892",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:11:19 2026 \u002B0200",
        "message": "chore: publish to nuget.org via trusted publishing (#1756)"
      },
      {
        "sha": "3fd1189bbcc7dcb05cc18646d823f18924ce7e17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:17:53 2026 \u002B0200",
        "message": "feat: add subject contexts to the expectation builder (#1755)"
      },
      {
        "sha": "fe613c6c5bb8baf0e41ddad6c7f2af5805bbdc64",
        "author": "Valentin Breu\u00DF",
        "date": "Fri Oct 9 06:15:39 2026 \u002B0200",
        "message": "docs: address review feedback on configuration, date/time and equivalency pages (#1757)"
      },
      {
        "sha": "5ef58e1ecd62ea196c69ed0946ef275e6cf8454e",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 08:39:58 2026 \u002B0200",
        "message": "chore: bump aweXpect.Core to v3.0.0 (#1777)"
      },
      {
        "sha": "28acb579dcb9a53f2fdd4cb58dbbf5dce70d4d5f",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 10:34:33 2026 \u002B0200",
        "message": "docs: clarify the once-only rule for extension options and past-tense naming for recorded occurrences (#1778)"
      },
      {
        "sha": "75406bf0523ab761f42b1ce387c81eede7daf1e6",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:26:40 2026 \u002B0200",
        "message": "chore: ship the v3.0.0 API (#1779)"
      },
      {
        "sha": "fa595d4790a49f64d5cf9e958b216ba40f17c5ad",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:27:33 2026 \u002B0200",
        "message": "docs: lead the \u0022Why aweXpect?\u0022 tabs with the extension packages (#1780)"
      },
      {
        "sha": "5cf6a80d0462504a666c7b204cded9d12cec0512",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 08:48:31 2026 \u002B0200",
        "message": "docs: show a consumer-side \u0060AndIf\u0060 for expectations that only apply under a condition (#1781)"
      },
      {
        "sha": "29e1127b443bc230edabceb08a728d9198753e20",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:26:33 2026 \u002B0200",
        "message": "perf: skip the repeated check in \u0060Satisfies\u0060 and \u0060CompliesWith\u0060 without a timeout (#1782)"
      },
      {
        "sha": "779048624672829223d4e1de669d23b41bf06597",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:39:04 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1783)"
      },
      {
        "sha": "c9eb2cefda18397f58824b92a06bbcd9b8261914",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:51:34 2026 \u002B0200",
        "message": "perf: skip the failure texts of a met \u0060InAnyOrder().IgnoringDuplicates()\u0060 (#1789)"
      },
      {
        "sha": "79d6813aeaac4319dfdcb670e428b84b6bd5693e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:52:06 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1790)"
      },
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "647847aa",
      "272bbb23",
      "23b6cd56",
      "a1b234c6",
      "168dc965",
      "f21e6eab",
      "d98a48a7",
      "0c874997",
      "0d6d920b",
      "c8e40f00",
      "a8e6cbb0",
      "82b90d2e",
      "e0c39d6f",
      "3a0d86a4",
      "e4877b71",
      "e4e4a0e6",
      "9f96f8b9",
      "a07a99e2",
      "998fbcba",
      "464696d1",
      "1f967b58",
      "7e4f4bdb",
      "93d22f80",
      "8b666ac8",
      "8c8f6bf6",
      "6242c56e",
      "f7a0873f",
      "8e2da4f3",
      "41cb59cc",
      "75cde8b0",
      "3fd1189b",
      "fe613c6c",
      "5ef58e1e",
      "28acb579",
      "75406bf0",
      "fa595d47",
      "5cf6a80d",
      "29e1127b",
      "77904862",
      "c9eb2cef",
      "79d6813a",
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          136.18030888693673,
          141.3437429024623,
          142.3776572863261,
          121.97559043566386,
          146.34538300832114,
          75.11130221400943,
          75.87682268449238,
          148.91186265945436,
          144.04427870114645,
          163.36314128239948,
          166.96671911875407,
          152.45862007141113,
          141.09372336069742,
          148.26110469500225,
          144.20769170125325,
          146.50328726768493,
          182.17733670870464,
          85.12685470921653,
          151.41907165845234,
          162.9340720006398,
          121.1920941727502,
          74.93121902759259,
          87.66823535760244,
          161.0230097611745,
          78.44054555098215,
          76.55528612647738,
          142.71374247755324,
          151.40262173016865,
          149.0776209036509,
          160.85706199010212,
          150.09303159713744,
          159.1996108373006,
          116.29780764239175,
          154.54549403190612,
          119.91555163065593,
          78.89571855862935,
          134.65263926188152,
          143.54727857907613,
          143.75318296750388,
          167.86962395906448,
          85.4657245238622,
          141.4626628081004,
          164.36755086580914,
          178.16398259798686,
          149.4230346361796,
          92.09907669287462,
          159.01998606749945,
          119.79168168703715,
          102.63410041332244,
          145.90668115615844
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
          512,
          512,
          512,
          512,
          512,
          512,
          512,
          512,
          512,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          520,
          528,
          528,
          528,
          528,
          528,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          544,
          512,
          512
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
          243.7638168334961,
          235.86193222658974,
          244.92914561430612,
          206.36891317367554,
          263.06252940495807,
          126.30781677563985,
          133.5051192442576,
          260.5635449545724,
          238.43588604245866,
          289.0748630932399,
          298.81401615142823,
          269.2316794054849,
          241.46723055839539,
          244.63455235163372,
          269.95344088872275,
          235.27648987088884,
          344.28317203521726,
          136.4304206053416,
          246.45414049284798,
          263.02980254246637,
          209.42606624762217,
          129.6808660541262,
          148.78025673230488,
          298.95889972050986,
          130.74108481407166,
          128.2575772660119,
          245.96520824432372,
          261.30135504404706,
          255.3344995498657,
          281.25273551940916,
          260.12614444096886,
          275.2909729003906,
          192.52232818603517,
          258.54078238351,
          191.88036661148072,
          126.45058309237162,
          216.3354002067021,
          244.9393288067409,
          243.4368403752645,
          301.0225156466166,
          142.64803654806954,
          245.0253323827471,
          292.4559158325195,
          313.91244951883954,
          243.18680463518416,
          147.03668247736417,
          255.07114610671997,
          204.0382374048233,
          183.83041546344757,
          253.06234607696533
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
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224,
          1224
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          235.56220450401307,
          230.35924092928568,
          245.47175674438478,
          200.6444193805967,
          257.78513463338214,
          118.24768235133244,
          123.74599652630943,
          245.37986879348756,
          232.59543402989706,
          266.55420713424684,
          276.0447735468546,
          249.01944433848064,
          234.01756398494427,
          237.19669600895472,
          200.84191783836908,
          235.24980303219385,
          268.45120941797893,
          131.01223762218768,
          236.13340884844462,
          244.23050106488742,
          188.14628766377766,
          122.3559182541711,
          140.24854431549707,
          232.68214756647745,
          126.82758138974508,
          124.99726145267486,
          244.97244342168173,
          256.1360109329224,
          244.33444571495056,
          260.1941842714945,
          257.2019051483699,
          260.9803979056222,
          186.36010232993536,
          236.85818397204082,
          196.27047096888225,
          122.47751767294747,
          212.70499805041723,
          233.72915994326274,
          236.03266569284293,
          221.5394665002823,
          133.42354708451492,
          239.25506162643433,
          263.9202355997903,
          237.01754932403566,
          233.4705132842064,
          147.38542197545368,
          251.19252502123516,
          202.0940637077604,
          140.15554475784302,
          245.82162682215372
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776,
          776
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "String": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          345.19680611292523,
          294.4670040448507,
          293.9115686098734,
          258.15258267947604,
          163.601525919778,
          300.61422910690305,
          202.27120525042216,
          170.37307264123643,
          284.33359142450183
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
          608,
          608,
          608,
          608,
          608,
          608,
          608,
          576,
          576
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
          534.5129611151559,
          532.5699252348679,
          648.3111619949341,
          510.0246894200643,
          316.5562982899802,
          542.7052757898966,
          412.9906762327467,
          413.9302067756653,
          545.3726666768392
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
          2304,
          2304,
          2304,
          2304,
          2304,
          2304,
          2304,
          2304,
          2304
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          333.64402395884196,
          347.73521010080975,
          338.2037599245707,
          344.12981658715466,
          190.8458044687907,
          343.9997938156128,
          276.31743211746215,
          221.40054513726915,
          339.0808546702067
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          912,
          912,
          912,
          912,
          912,
          912,
          912,
          912,
          912
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "StringArray": {
    "commits": [
      {
        "sha": "647847aae6bff0b87edd4fb2b0f31ea389537e57",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:31 2026 \u002B0200",
        "message": "fix: enumerate the expected sequence of collection \u0060IsEqualTo\u0060 and \u0060IsContainedIn\u0060 only once per expectation (#1668)"
      },
      {
        "sha": "272bbb23db0b17bf4bfdffd1a5431707dacffa73",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:59 2026 \u002B0200",
        "message": "fix: skip every type that is compared by value or by content in the metadata generator (#1667)"
      },
      {
        "sha": "23b6cd56dbd45d5be7a56d4d2fc91ba0f1b6fba1",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:15:43 2026 \u002B0200",
        "message": "fix: accept finite numbers whose distance overflows in strict ordering with an infinite tolerance (#1666)"
      },
      {
        "sha": "a1b234c6becec264c0e5affca08319172f105a7c",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:16:17 2026 \u002B0200",
        "message": "fix!: do not offer a second \u0060Matching\u0060 on the result of \u0060HasSingle().Matching\u2026\u0060 (#1663)"
      },
      {
        "sha": "168dc965e6b1c84d256ac40eb02d77d52b142043",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 22:13:55 2026 \u002B0200",
        "message": "fix!: reject a property expression of \u0060TriggeredPropertyChangedFor\u0060 that is no property of the subject (#1665)"
      },
      {
        "sha": "f21e6eaba602e779f0efe0ae83e392121e673f0e",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 06:36:57 2026 \u002B0200",
        "message": "refactor: make the timeout and retry tests independent of the machine speed (#1658)"
      },
      {
        "sha": "d98a48a7c9027a5e9a3fb8092b2b7eb259848f11",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 07:20:25 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.16 (#1670)"
      },
      {
        "sha": "0c874997620abfae9aec2928a18f9bffbd2ee840",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 08:34:19 2026 \u002B0200",
        "message": "refactor: reduce the cognitive complexity of \u0060EmitRegistration\u0060 in the \u0060TypeMetadataGenerator\u0060 (#1671)"
      },
      {
        "sha": "0d6d920b6328f9cb9a22fe6e17b1c17be8dd5cfa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 18:34:38 2026 \u002B0200",
        "message": "test: migrate the unit tests from xUnit v2 to TUnit (#1672)"
      },
      {
        "sha": "c8e40f0059cc58502f272fc46bb6c8133187ecb3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:07:05 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.17 (#1687)"
      },
      {
        "sha": "a8e6cbb0921667eb9773fa25913c69b0c4430849",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:17:05 2026 \u002B0200",
        "message": "test: make real-time dependent tests of \u0060Eventually\u0060, \u0060Polling\u0060 and the signaler deterministic (#1686)"
      },
      {
        "sha": "82b90d2ea7e6b0152df0739bbbee5fdaada69a39",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:50:55 2026 \u002B0200",
        "message": "ci: identify the pull request of an analysed run by the event instead of its artifacts (#1688)"
      },
      {
        "sha": "e0c39d6f3fd9d409274b60c5a932bada9f55a9c0",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:40:10 2026 \u002B0200",
        "message": "fix: describe the last attempt in \u0060IsOneOf\u0060 and \u0060IsNotOneOf\u0060 on \u0060DateTime\u0060 after an earlier incompatible kind (#1689)"
      },
      {
        "sha": "3a0d86a45c60afc2bdef37100570c1e1ac13bddc",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:53:09 2026 \u002B0200",
        "message": "ci: disable the mutation tests and pin the Stryker version (#1693)"
      },
      {
        "sha": "e4877b71814748aeb8eae0bfbd3698446d51a28a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:11:36 2026 \u002B0200",
        "message": "test: pin the NUnit 4 adapter tests to NUnit 4 and add a project for NUnit 5 (#1690)"
      },
      {
        "sha": "e4e4a0e6d5366735cd810bd521c7f2cc8e8202c1",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:39:49 2026 \u002B0200",
        "message": "fix!: hand out a non-null subject when awaiting \u0060IsParsableInto\u0060 on a string (#1691)"
      },
      {
        "sha": "9f96f8b98b0347930a55827ba71f5ac3a27432eb",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:42:31 2026 \u002B0200",
        "message": "ci: close gaps in the test runs and the public API check of pull requests (#1692)"
      },
      {
        "sha": "a07a99e21c62fcc44d71c973dc607c32b262670a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 15:25:38 2026 \u002B0200",
        "message": "fix: bound the check of an attempt of \u0060Eventually()\u0060 by the retry budget (#1695)"
      },
      {
        "sha": "998fbcba07f55588650d37e481c620b6925b626d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:09:27 2026 \u002B0200",
        "message": "fix: fail the expectation when the enumerator or the current item of a materialized subject throws (#1697)"
      },
      {
        "sha": "464696d177a9f9c12b445c4c827f1ac2a073c091",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:11:22 2026 \u002B0200",
        "message": "fix: keep the outcome of a quantified expectation when user code throws after the deciding item of a collection with a known count (#1698)"
      },
      {
        "sha": "1f967b5891c9d2474c74e7122df47418ba35c8c0",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 12:19:03 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.18 (#1740)"
      },
      {
        "sha": "7e4f4bdba98cfa1eddad59a18b83cbdf776c174d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:14:00 2026 \u002B0200",
        "message": "fix: show the whole string difference when the minimum number of characters after it is close to \u0060int.MaxValue\u0060 (#1741)"
      },
      {
        "sha": "93d22f80c1c71b9006b4950886cc204fc899957c",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:23:23 2026 \u002B0200",
        "message": "fix: do not suppress nullability warnings when the statement of the expectation overwrites the subject (#1742)"
      },
      {
        "sha": "8b666ac8f8b63072a96fd3152182e03b2cd192f8",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:31:43 2026 \u002B0200",
        "message": "fix!: check the kinds of \u0060DateTime\u0060 values in an untyped collection when verifying its order (#1744)"
      },
      {
        "sha": "8c8f6bf6b6495d38ccf74cdd4c25ae541f0b3e6d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:36:07 2026 \u002B0200",
        "message": "fix: keep an event recording running across the repeated checks of \u0060CompliesWith(\u2026).Within(\u2026)\u0060 (#1745)"
      },
      {
        "sha": "6242c56e2ef28022bdbd22a141f2dd24a3d30374",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:50:50 2026 \u002B0200",
        "message": "fix!: pass a null argument to the predicate of the positional event filters (#1747)"
      },
      {
        "sha": "f7a0873ff8b24ee55e1f59c3990cf1ab57f61be6",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 16:06:38 2026 \u002B0200",
        "message": "fix: stop reading the source for the failure message of the synchronous \u0060IsEmpty\u0060 (#1746)"
      },
      {
        "sha": "8e2da4f371eda53d393d9e80ba7a07a78b9b2905",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:01:52 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.19 (#1753)"
      },
      {
        "sha": "41cb59cca90ed6a485f0c71418670b19988dc75b",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:34:01 2026 \u002B0200",
        "message": "docs: shorten the v3 migration guide and trim the reference pages (#1754)"
      },
      {
        "sha": "75cde8b0fa6fde9400552cb23a296a087933c892",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:11:19 2026 \u002B0200",
        "message": "chore: publish to nuget.org via trusted publishing (#1756)"
      },
      {
        "sha": "3fd1189bbcc7dcb05cc18646d823f18924ce7e17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:17:53 2026 \u002B0200",
        "message": "feat: add subject contexts to the expectation builder (#1755)"
      },
      {
        "sha": "fe613c6c5bb8baf0e41ddad6c7f2af5805bbdc64",
        "author": "Valentin Breu\u00DF",
        "date": "Fri Oct 9 06:15:39 2026 \u002B0200",
        "message": "docs: address review feedback on configuration, date/time and equivalency pages (#1757)"
      },
      {
        "sha": "5ef58e1ecd62ea196c69ed0946ef275e6cf8454e",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 08:39:58 2026 \u002B0200",
        "message": "chore: bump aweXpect.Core to v3.0.0 (#1777)"
      },
      {
        "sha": "28acb579dcb9a53f2fdd4cb58dbbf5dce70d4d5f",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 10:34:33 2026 \u002B0200",
        "message": "docs: clarify the once-only rule for extension options and past-tense naming for recorded occurrences (#1778)"
      },
      {
        "sha": "75406bf0523ab761f42b1ce387c81eede7daf1e6",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:26:40 2026 \u002B0200",
        "message": "chore: ship the v3.0.0 API (#1779)"
      },
      {
        "sha": "fa595d4790a49f64d5cf9e958b216ba40f17c5ad",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:27:33 2026 \u002B0200",
        "message": "docs: lead the \u0022Why aweXpect?\u0022 tabs with the extension packages (#1780)"
      },
      {
        "sha": "5cf6a80d0462504a666c7b204cded9d12cec0512",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 08:48:31 2026 \u002B0200",
        "message": "docs: show a consumer-side \u0060AndIf\u0060 for expectations that only apply under a condition (#1781)"
      },
      {
        "sha": "29e1127b443bc230edabceb08a728d9198753e20",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:26:33 2026 \u002B0200",
        "message": "perf: skip the repeated check in \u0060Satisfies\u0060 and \u0060CompliesWith\u0060 without a timeout (#1782)"
      },
      {
        "sha": "779048624672829223d4e1de669d23b41bf06597",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:39:04 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1783)"
      },
      {
        "sha": "c9eb2cefda18397f58824b92a06bbcd9b8261914",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:51:34 2026 \u002B0200",
        "message": "perf: skip the failure texts of a met \u0060InAnyOrder().IgnoringDuplicates()\u0060 (#1789)"
      },
      {
        "sha": "79d6813aeaac4319dfdcb670e428b84b6bd5693e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:52:06 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1790)"
      },
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "647847aa",
      "272bbb23",
      "23b6cd56",
      "a1b234c6",
      "168dc965",
      "f21e6eab",
      "d98a48a7",
      "0c874997",
      "0d6d920b",
      "c8e40f00",
      "a8e6cbb0",
      "82b90d2e",
      "e0c39d6f",
      "3a0d86a4",
      "e4877b71",
      "e4e4a0e6",
      "9f96f8b9",
      "a07a99e2",
      "998fbcba",
      "464696d1",
      "1f967b58",
      "7e4f4bdb",
      "93d22f80",
      "8b666ac8",
      "8c8f6bf6",
      "6242c56e",
      "f7a0873f",
      "8e2da4f3",
      "41cb59cc",
      "75cde8b0",
      "3fd1189b",
      "fe613c6c",
      "5ef58e1e",
      "28acb579",
      "75406bf0",
      "fa595d47",
      "5cf6a80d",
      "29e1127b",
      "77904862",
      "c9eb2cef",
      "79d6813a",
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          640.4088826497396,
          593.7735684076945,
          611.4669560750325,
          477.85231691996256,
          612.2908625284831,
          303.5208943230765,
          296.8572865486145,
          640.9725953420003,
          594.1570999962943,
          649.4298574447632,
          688.6662399927775,
          641.1165024893625,
          590.6863476679875,
          577.3890611784799,
          529.6957696914673,
          591.1264041491917,
          690.1892388661703,
          342.1757644812266,
          596.966326268514,
          566.4970643860953,
          497.140402730306,
          336.2688202176775,
          346.20272432054793,
          638.4499005590167,
          348.4767696380615,
          337.06756407873974,
          641.2353713853018,
          636.2342558542888,
          664.0919696172078,
          652.825014591217,
          669.0740095547268,
          642.7825833002727,
          495.1894598007202,
          641.9548854192097,
          503.2129665374756,
          345.8838263193766,
          595.3918642316546,
          658.5875039418538,
          709.4810994466146,
          567.9119215692792,
          319.78174807475165,
          661.8102690832956,
          683.081659243657,
          576.2982899347941,
          627.9572465079171,
          341.3065070424761,
          651.3049010594685,
          499.8275589261736,
          377.7996959319481,
          649.0716806139264
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
          840,
          840,
          840,
          840,
          840,
          840,
          840,
          840,
          840,
          856,
          856,
          856,
          856,
          856,
          856,
          856,
          856,
          856,
          856,
          856,
          872,
          872,
          872,
          872,
          872,
          872,
          872,
          880,
          880,
          880,
          880,
          880,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          896,
          864,
          864
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
          1320.280786259969,
          1326.0671399434407,
          1364.6358740488688,
          1118.0606801350912,
          1351.6633270263671,
          640.0229090963091,
          609.1682336330414,
          1371.2204779307046,
          1255.9418339362512,
          1421.3493090311686,
          1516.7950865427654,
          1356.2710932413736,
          1274.0322651181903,
          1284.0766259511313,
          1272.2204156239827,
          1241.7527656555176,
          1588.7325370788574,
          708.1604086330959,
          1361.6681577046713,
          1277.5239940370832,
          1056.205288205828,
          674.7569321950276,
          794.6561574935913,
          1432.9326642354329,
          678.3175195966448,
          725.0460067431133,
          1429.9953777313233,
          1378.3177547454834,
          1382.3861387797765,
          1404.0230564117433,
          1410.129246266683,
          1380.0243453979492,
          1017.1524626413981,
          1273.6667727152505,
          1033.9537096659342,
          731.7685546875,
          1130.4489125569662,
          1426.0169359353872,
          1730.979863357544,
          1472.4219537462507,
          653.8707575480144,
          1273.0741392771404,
          1370.1992055257163,
          1454.9722957611084,
          1332.0978571573894,
          746.8850107192993,
          1377.3765979494367,
          1093.4844435283117,
          915.3745296918429,
          1393.0922468821207
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
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152,
          4152
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          611.9931491851806,
          610.7512573401133,
          633.0534842173258,
          532.8129401524861,
          655.5329192968516,
          352.2561849753062,
          306.9008693013872,
          646.291883913676,
          606.9482201848712,
          671.4865087509155,
          700.1051335016887,
          680.0415073394776,
          621.9136136599949,
          634.1302262714931,
          552.827801322937,
          613.0097901026407,
          713.7595713933309,
          362.3581245128925,
          650.306664721171,
          636.9033426871666,
          488.7000308354696,
          327.1852085249765,
          365.86587247848513,
          742.2957029342651,
          334.13146339144026,
          331.785920826594,
          629.0753513336182,
          653.5006825129191,
          648.7793511549631,
          661.3155122756958,
          657.2392098108927,
          645.1081802504403,
          504.1748679478963,
          634.1702184041341,
          517.6308202062335,
          343.8891352585384,
          580.1355312892368,
          629.9172812143962,
          627.3235552469889,
          613.8913120542254,
          313.5640004793803,
          623.5576110567365,
          672.1235249201457,
          621.1164381027222,
          629.9775292323186,
          366.1082080523173,
          645.6157728195191,
          511.57717418670654,
          409.44116151332855,
          660.1025011062623
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "StringArrayInAnyOrder": {
    "commits": [
      {
        "sha": "647847aae6bff0b87edd4fb2b0f31ea389537e57",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:31 2026 \u002B0200",
        "message": "fix: enumerate the expected sequence of collection \u0060IsEqualTo\u0060 and \u0060IsContainedIn\u0060 only once per expectation (#1668)"
      },
      {
        "sha": "272bbb23db0b17bf4bfdffd1a5431707dacffa73",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:59 2026 \u002B0200",
        "message": "fix: skip every type that is compared by value or by content in the metadata generator (#1667)"
      },
      {
        "sha": "23b6cd56dbd45d5be7a56d4d2fc91ba0f1b6fba1",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:15:43 2026 \u002B0200",
        "message": "fix: accept finite numbers whose distance overflows in strict ordering with an infinite tolerance (#1666)"
      },
      {
        "sha": "a1b234c6becec264c0e5affca08319172f105a7c",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:16:17 2026 \u002B0200",
        "message": "fix!: do not offer a second \u0060Matching\u0060 on the result of \u0060HasSingle().Matching\u2026\u0060 (#1663)"
      },
      {
        "sha": "168dc965e6b1c84d256ac40eb02d77d52b142043",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 22:13:55 2026 \u002B0200",
        "message": "fix!: reject a property expression of \u0060TriggeredPropertyChangedFor\u0060 that is no property of the subject (#1665)"
      },
      {
        "sha": "f21e6eaba602e779f0efe0ae83e392121e673f0e",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 06:36:57 2026 \u002B0200",
        "message": "refactor: make the timeout and retry tests independent of the machine speed (#1658)"
      },
      {
        "sha": "d98a48a7c9027a5e9a3fb8092b2b7eb259848f11",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 07:20:25 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.16 (#1670)"
      },
      {
        "sha": "0c874997620abfae9aec2928a18f9bffbd2ee840",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 08:34:19 2026 \u002B0200",
        "message": "refactor: reduce the cognitive complexity of \u0060EmitRegistration\u0060 in the \u0060TypeMetadataGenerator\u0060 (#1671)"
      },
      {
        "sha": "0d6d920b6328f9cb9a22fe6e17b1c17be8dd5cfa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 18:34:38 2026 \u002B0200",
        "message": "test: migrate the unit tests from xUnit v2 to TUnit (#1672)"
      },
      {
        "sha": "c8e40f0059cc58502f272fc46bb6c8133187ecb3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:07:05 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.17 (#1687)"
      },
      {
        "sha": "a8e6cbb0921667eb9773fa25913c69b0c4430849",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:17:05 2026 \u002B0200",
        "message": "test: make real-time dependent tests of \u0060Eventually\u0060, \u0060Polling\u0060 and the signaler deterministic (#1686)"
      },
      {
        "sha": "82b90d2ea7e6b0152df0739bbbee5fdaada69a39",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:50:55 2026 \u002B0200",
        "message": "ci: identify the pull request of an analysed run by the event instead of its artifacts (#1688)"
      },
      {
        "sha": "e0c39d6f3fd9d409274b60c5a932bada9f55a9c0",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:40:10 2026 \u002B0200",
        "message": "fix: describe the last attempt in \u0060IsOneOf\u0060 and \u0060IsNotOneOf\u0060 on \u0060DateTime\u0060 after an earlier incompatible kind (#1689)"
      },
      {
        "sha": "3a0d86a45c60afc2bdef37100570c1e1ac13bddc",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:53:09 2026 \u002B0200",
        "message": "ci: disable the mutation tests and pin the Stryker version (#1693)"
      },
      {
        "sha": "e4877b71814748aeb8eae0bfbd3698446d51a28a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:11:36 2026 \u002B0200",
        "message": "test: pin the NUnit 4 adapter tests to NUnit 4 and add a project for NUnit 5 (#1690)"
      },
      {
        "sha": "e4e4a0e6d5366735cd810bd521c7f2cc8e8202c1",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:39:49 2026 \u002B0200",
        "message": "fix!: hand out a non-null subject when awaiting \u0060IsParsableInto\u0060 on a string (#1691)"
      },
      {
        "sha": "9f96f8b98b0347930a55827ba71f5ac3a27432eb",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:42:31 2026 \u002B0200",
        "message": "ci: close gaps in the test runs and the public API check of pull requests (#1692)"
      },
      {
        "sha": "a07a99e21c62fcc44d71c973dc607c32b262670a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 15:25:38 2026 \u002B0200",
        "message": "fix: bound the check of an attempt of \u0060Eventually()\u0060 by the retry budget (#1695)"
      },
      {
        "sha": "998fbcba07f55588650d37e481c620b6925b626d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:09:27 2026 \u002B0200",
        "message": "fix: fail the expectation when the enumerator or the current item of a materialized subject throws (#1697)"
      },
      {
        "sha": "464696d177a9f9c12b445c4c827f1ac2a073c091",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:11:22 2026 \u002B0200",
        "message": "fix: keep the outcome of a quantified expectation when user code throws after the deciding item of a collection with a known count (#1698)"
      },
      {
        "sha": "1f967b5891c9d2474c74e7122df47418ba35c8c0",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 12:19:03 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.18 (#1740)"
      },
      {
        "sha": "7e4f4bdba98cfa1eddad59a18b83cbdf776c174d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:14:00 2026 \u002B0200",
        "message": "fix: show the whole string difference when the minimum number of characters after it is close to \u0060int.MaxValue\u0060 (#1741)"
      },
      {
        "sha": "93d22f80c1c71b9006b4950886cc204fc899957c",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:23:23 2026 \u002B0200",
        "message": "fix: do not suppress nullability warnings when the statement of the expectation overwrites the subject (#1742)"
      },
      {
        "sha": "8b666ac8f8b63072a96fd3152182e03b2cd192f8",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:31:43 2026 \u002B0200",
        "message": "fix!: check the kinds of \u0060DateTime\u0060 values in an untyped collection when verifying its order (#1744)"
      },
      {
        "sha": "8c8f6bf6b6495d38ccf74cdd4c25ae541f0b3e6d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:36:07 2026 \u002B0200",
        "message": "fix: keep an event recording running across the repeated checks of \u0060CompliesWith(\u2026).Within(\u2026)\u0060 (#1745)"
      },
      {
        "sha": "6242c56e2ef28022bdbd22a141f2dd24a3d30374",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:50:50 2026 \u002B0200",
        "message": "fix!: pass a null argument to the predicate of the positional event filters (#1747)"
      },
      {
        "sha": "f7a0873ff8b24ee55e1f59c3990cf1ab57f61be6",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 16:06:38 2026 \u002B0200",
        "message": "fix: stop reading the source for the failure message of the synchronous \u0060IsEmpty\u0060 (#1746)"
      },
      {
        "sha": "8e2da4f371eda53d393d9e80ba7a07a78b9b2905",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:01:52 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.19 (#1753)"
      },
      {
        "sha": "41cb59cca90ed6a485f0c71418670b19988dc75b",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:34:01 2026 \u002B0200",
        "message": "docs: shorten the v3 migration guide and trim the reference pages (#1754)"
      },
      {
        "sha": "75cde8b0fa6fde9400552cb23a296a087933c892",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:11:19 2026 \u002B0200",
        "message": "chore: publish to nuget.org via trusted publishing (#1756)"
      },
      {
        "sha": "3fd1189bbcc7dcb05cc18646d823f18924ce7e17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:17:53 2026 \u002B0200",
        "message": "feat: add subject contexts to the expectation builder (#1755)"
      },
      {
        "sha": "fe613c6c5bb8baf0e41ddad6c7f2af5805bbdc64",
        "author": "Valentin Breu\u00DF",
        "date": "Fri Oct 9 06:15:39 2026 \u002B0200",
        "message": "docs: address review feedback on configuration, date/time and equivalency pages (#1757)"
      },
      {
        "sha": "5ef58e1ecd62ea196c69ed0946ef275e6cf8454e",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 08:39:58 2026 \u002B0200",
        "message": "chore: bump aweXpect.Core to v3.0.0 (#1777)"
      },
      {
        "sha": "28acb579dcb9a53f2fdd4cb58dbbf5dce70d4d5f",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 10:34:33 2026 \u002B0200",
        "message": "docs: clarify the once-only rule for extension options and past-tense naming for recorded occurrences (#1778)"
      },
      {
        "sha": "75406bf0523ab761f42b1ce387c81eede7daf1e6",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:26:40 2026 \u002B0200",
        "message": "chore: ship the v3.0.0 API (#1779)"
      },
      {
        "sha": "fa595d4790a49f64d5cf9e958b216ba40f17c5ad",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:27:33 2026 \u002B0200",
        "message": "docs: lead the \u0022Why aweXpect?\u0022 tabs with the extension packages (#1780)"
      },
      {
        "sha": "5cf6a80d0462504a666c7b204cded9d12cec0512",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 08:48:31 2026 \u002B0200",
        "message": "docs: show a consumer-side \u0060AndIf\u0060 for expectations that only apply under a condition (#1781)"
      },
      {
        "sha": "29e1127b443bc230edabceb08a728d9198753e20",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:26:33 2026 \u002B0200",
        "message": "perf: skip the repeated check in \u0060Satisfies\u0060 and \u0060CompliesWith\u0060 without a timeout (#1782)"
      },
      {
        "sha": "779048624672829223d4e1de669d23b41bf06597",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:39:04 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1783)"
      },
      {
        "sha": "c9eb2cefda18397f58824b92a06bbcd9b8261914",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:51:34 2026 \u002B0200",
        "message": "perf: skip the failure texts of a met \u0060InAnyOrder().IgnoringDuplicates()\u0060 (#1789)"
      },
      {
        "sha": "79d6813aeaac4319dfdcb670e428b84b6bd5693e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:52:06 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1790)"
      },
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "647847aa",
      "272bbb23",
      "23b6cd56",
      "a1b234c6",
      "168dc965",
      "f21e6eab",
      "d98a48a7",
      "0c874997",
      "0d6d920b",
      "c8e40f00",
      "a8e6cbb0",
      "82b90d2e",
      "e0c39d6f",
      "3a0d86a4",
      "e4877b71",
      "e4e4a0e6",
      "9f96f8b9",
      "a07a99e2",
      "998fbcba",
      "464696d1",
      "1f967b58",
      "7e4f4bdb",
      "93d22f80",
      "8b666ac8",
      "8c8f6bf6",
      "6242c56e",
      "f7a0873f",
      "8e2da4f3",
      "41cb59cc",
      "75cde8b0",
      "3fd1189b",
      "fe613c6c",
      "5ef58e1e",
      "28acb579",
      "75406bf0",
      "fa595d47",
      "5cf6a80d",
      "29e1127b",
      "77904862",
      "c9eb2cef",
      "79d6813a",
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1053.141568844135,
          975.4341665903727,
          973.5896180959849,
          812.7235339028495,
          972.0514018195016,
          507.56434903826033,
          502.9044852623573,
          969.7287312825521,
          943.9315375010173,
          1023.2954066140311,
          1056.9482728413172,
          1000.4291741689046,
          928.3969171524047,
          967.2160835266113,
          826.6514413197835,
          970.0448721476963,
          1093.7553983052571,
          551.4206887880961,
          1034.52440478007,
          990.2430680138724,
          798.7452133618868,
          524.0213324106657,
          567.3830805558425,
          1065.7342980248588,
          547.7202320098877,
          515.1199394634792,
          1046.7737821851458,
          1051.0342363993327,
          1060.7046702067057,
          1036.0475843974523,
          1016.0087453297207,
          1116.2271496909004,
          784.9643902778625,
          1051.5613887786865,
          788.6019387563069,
          562.4833726565043,
          920.6372468130929,
          1007.0624073028564,
          1060.3769185384115,
          896.5114022663662,
          501.74725475311277,
          1014.9841235024588,
          1088.6417965207781,
          870.4012481689454,
          1006.2845067342122,
          566.3653113047282,
          1004.2887246449789,
          828.8989026387533,
          392.56831844036395,
          655.6204875310262
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
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1264,
          1280,
          1280,
          1280,
          1280,
          1280,
          1280,
          1280,
          1288,
          1288,
          1288,
          1288,
          1288,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          1304,
          912,
          912
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
          26669.563818359376,
          26816.98806966146,
          27361.74025065104,
          16816.55473429362,
          27497.089080810547,
          10018.345930916923,
          9443.415021623883,
          27719.161811241738,
          26995.21945659931,
          28227.228971354165,
          28796.494430541992,
          27469.12569173177,
          26178.600689697265,
          20204.395196097237,
          17054.773201497395,
          25491.593420846122,
          25337.29578944615,
          10919.010286603656,
          20865.700933837892,
          19326.512987264,
          15555.621950276693,
          10848.702115885417,
          10781.839710489909,
          24206.77577311198,
          10362.304801354041,
          10069.346013750348,
          27037.291407658504,
          27659.082942708334,
          27789.579213460285,
          27858.206268310547,
          27096.974322509766,
          28014.6536325308,
          15738.152657645089,
          27330.002638596754,
          15811.865844726562,
          10708.345770263672,
          23787.845112391882,
          26538.971545410157,
          27034.31483561198,
          17750.907584635417,
          9782.237326049804,
          27072.115968831382,
          27906.213224283852,
          17623.03741455078,
          19363.912045992336,
          10944.914141845704,
          27059.849483235677,
          16400.73519083659,
          11769.582413991293,
          27887.573141915458
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
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33465,
          33471,
          33468,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33468,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33471,
          33465,
          33471,
          33471,
          33471,
          33465,
          33471,
          33471,
          33471,
          33471,
          33465,
          33471
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          753.8996606554304,
          780.9781115849813,
          774.389230455671,
          621.0810632024493,
          808.3331483840942,
          427.1614917755127,
          374.5448865890503,
          808.6336347579957,
          757.7921792439053,
          792.4907321248736,
          860.9113993962605,
          795.6614363988241,
          743.0316321690877,
          732.0670407613119,
          677.7818338530404,
          763.0835143602811,
          923.8292442321778,
          434.2536027772086,
          735.9685022989909,
          724.3659522874015,
          573.0873628616333,
          414.6715227762858,
          458.7878606160482,
          902.8659006265493,
          404.62155400003707,
          406.55301560674394,
          771.6197384425571,
          811.5114925384521,
          778.4879866282146,
          812.0843548456828,
          789.7045337677002,
          799.8754990259806,
          584.2545240402221,
          772.756406656901,
          596.6405504090445,
          401.6827799592699,
          709.1366493361337,
          781.367879931132,
          742.5795537948609,
          751.286527188619,
          381.7459143570491,
          753.1684947695051,
          850.9522219340007,
          723.5283984456744,
          723.1027789433797,
          444.03315149034773,
          795.810476175944,
          625.2940127690633,
          482.3008575439453,
          804.692634264628
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408,
          1408
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "EventTriggered": {
    "commits": [
      {
        "sha": "647847aae6bff0b87edd4fb2b0f31ea389537e57",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:31 2026 \u002B0200",
        "message": "fix: enumerate the expected sequence of collection \u0060IsEqualTo\u0060 and \u0060IsContainedIn\u0060 only once per expectation (#1668)"
      },
      {
        "sha": "272bbb23db0b17bf4bfdffd1a5431707dacffa73",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:59 2026 \u002B0200",
        "message": "fix: skip every type that is compared by value or by content in the metadata generator (#1667)"
      },
      {
        "sha": "23b6cd56dbd45d5be7a56d4d2fc91ba0f1b6fba1",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:15:43 2026 \u002B0200",
        "message": "fix: accept finite numbers whose distance overflows in strict ordering with an infinite tolerance (#1666)"
      },
      {
        "sha": "a1b234c6becec264c0e5affca08319172f105a7c",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:16:17 2026 \u002B0200",
        "message": "fix!: do not offer a second \u0060Matching\u0060 on the result of \u0060HasSingle().Matching\u2026\u0060 (#1663)"
      },
      {
        "sha": "168dc965e6b1c84d256ac40eb02d77d52b142043",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 22:13:55 2026 \u002B0200",
        "message": "fix!: reject a property expression of \u0060TriggeredPropertyChangedFor\u0060 that is no property of the subject (#1665)"
      },
      {
        "sha": "f21e6eaba602e779f0efe0ae83e392121e673f0e",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 06:36:57 2026 \u002B0200",
        "message": "refactor: make the timeout and retry tests independent of the machine speed (#1658)"
      },
      {
        "sha": "d98a48a7c9027a5e9a3fb8092b2b7eb259848f11",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 07:20:25 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.16 (#1670)"
      },
      {
        "sha": "0c874997620abfae9aec2928a18f9bffbd2ee840",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 08:34:19 2026 \u002B0200",
        "message": "refactor: reduce the cognitive complexity of \u0060EmitRegistration\u0060 in the \u0060TypeMetadataGenerator\u0060 (#1671)"
      },
      {
        "sha": "0d6d920b6328f9cb9a22fe6e17b1c17be8dd5cfa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 18:34:38 2026 \u002B0200",
        "message": "test: migrate the unit tests from xUnit v2 to TUnit (#1672)"
      },
      {
        "sha": "c8e40f0059cc58502f272fc46bb6c8133187ecb3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:07:05 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.17 (#1687)"
      },
      {
        "sha": "a8e6cbb0921667eb9773fa25913c69b0c4430849",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:17:05 2026 \u002B0200",
        "message": "test: make real-time dependent tests of \u0060Eventually\u0060, \u0060Polling\u0060 and the signaler deterministic (#1686)"
      },
      {
        "sha": "82b90d2ea7e6b0152df0739bbbee5fdaada69a39",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:50:55 2026 \u002B0200",
        "message": "ci: identify the pull request of an analysed run by the event instead of its artifacts (#1688)"
      },
      {
        "sha": "e0c39d6f3fd9d409274b60c5a932bada9f55a9c0",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:40:10 2026 \u002B0200",
        "message": "fix: describe the last attempt in \u0060IsOneOf\u0060 and \u0060IsNotOneOf\u0060 on \u0060DateTime\u0060 after an earlier incompatible kind (#1689)"
      },
      {
        "sha": "3a0d86a45c60afc2bdef37100570c1e1ac13bddc",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:53:09 2026 \u002B0200",
        "message": "ci: disable the mutation tests and pin the Stryker version (#1693)"
      },
      {
        "sha": "e4877b71814748aeb8eae0bfbd3698446d51a28a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:11:36 2026 \u002B0200",
        "message": "test: pin the NUnit 4 adapter tests to NUnit 4 and add a project for NUnit 5 (#1690)"
      },
      {
        "sha": "e4e4a0e6d5366735cd810bd521c7f2cc8e8202c1",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:39:49 2026 \u002B0200",
        "message": "fix!: hand out a non-null subject when awaiting \u0060IsParsableInto\u0060 on a string (#1691)"
      },
      {
        "sha": "9f96f8b98b0347930a55827ba71f5ac3a27432eb",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:42:31 2026 \u002B0200",
        "message": "ci: close gaps in the test runs and the public API check of pull requests (#1692)"
      },
      {
        "sha": "a07a99e21c62fcc44d71c973dc607c32b262670a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 15:25:38 2026 \u002B0200",
        "message": "fix: bound the check of an attempt of \u0060Eventually()\u0060 by the retry budget (#1695)"
      },
      {
        "sha": "998fbcba07f55588650d37e481c620b6925b626d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:09:27 2026 \u002B0200",
        "message": "fix: fail the expectation when the enumerator or the current item of a materialized subject throws (#1697)"
      },
      {
        "sha": "464696d177a9f9c12b445c4c827f1ac2a073c091",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:11:22 2026 \u002B0200",
        "message": "fix: keep the outcome of a quantified expectation when user code throws after the deciding item of a collection with a known count (#1698)"
      },
      {
        "sha": "1f967b5891c9d2474c74e7122df47418ba35c8c0",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 12:19:03 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.18 (#1740)"
      },
      {
        "sha": "7e4f4bdba98cfa1eddad59a18b83cbdf776c174d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:14:00 2026 \u002B0200",
        "message": "fix: show the whole string difference when the minimum number of characters after it is close to \u0060int.MaxValue\u0060 (#1741)"
      },
      {
        "sha": "93d22f80c1c71b9006b4950886cc204fc899957c",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:23:23 2026 \u002B0200",
        "message": "fix: do not suppress nullability warnings when the statement of the expectation overwrites the subject (#1742)"
      },
      {
        "sha": "8b666ac8f8b63072a96fd3152182e03b2cd192f8",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:31:43 2026 \u002B0200",
        "message": "fix!: check the kinds of \u0060DateTime\u0060 values in an untyped collection when verifying its order (#1744)"
      },
      {
        "sha": "8c8f6bf6b6495d38ccf74cdd4c25ae541f0b3e6d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:36:07 2026 \u002B0200",
        "message": "fix: keep an event recording running across the repeated checks of \u0060CompliesWith(\u2026).Within(\u2026)\u0060 (#1745)"
      },
      {
        "sha": "6242c56e2ef28022bdbd22a141f2dd24a3d30374",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:50:50 2026 \u002B0200",
        "message": "fix!: pass a null argument to the predicate of the positional event filters (#1747)"
      },
      {
        "sha": "f7a0873ff8b24ee55e1f59c3990cf1ab57f61be6",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 16:06:38 2026 \u002B0200",
        "message": "fix: stop reading the source for the failure message of the synchronous \u0060IsEmpty\u0060 (#1746)"
      },
      {
        "sha": "8e2da4f371eda53d393d9e80ba7a07a78b9b2905",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:01:52 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.19 (#1753)"
      },
      {
        "sha": "41cb59cca90ed6a485f0c71418670b19988dc75b",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:34:01 2026 \u002B0200",
        "message": "docs: shorten the v3 migration guide and trim the reference pages (#1754)"
      },
      {
        "sha": "75cde8b0fa6fde9400552cb23a296a087933c892",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:11:19 2026 \u002B0200",
        "message": "chore: publish to nuget.org via trusted publishing (#1756)"
      },
      {
        "sha": "3fd1189bbcc7dcb05cc18646d823f18924ce7e17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:17:53 2026 \u002B0200",
        "message": "feat: add subject contexts to the expectation builder (#1755)"
      },
      {
        "sha": "fe613c6c5bb8baf0e41ddad6c7f2af5805bbdc64",
        "author": "Valentin Breu\u00DF",
        "date": "Fri Oct 9 06:15:39 2026 \u002B0200",
        "message": "docs: address review feedback on configuration, date/time and equivalency pages (#1757)"
      },
      {
        "sha": "5ef58e1ecd62ea196c69ed0946ef275e6cf8454e",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 08:39:58 2026 \u002B0200",
        "message": "chore: bump aweXpect.Core to v3.0.0 (#1777)"
      },
      {
        "sha": "28acb579dcb9a53f2fdd4cb58dbbf5dce70d4d5f",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 10:34:33 2026 \u002B0200",
        "message": "docs: clarify the once-only rule for extension options and past-tense naming for recorded occurrences (#1778)"
      },
      {
        "sha": "75406bf0523ab761f42b1ce387c81eede7daf1e6",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:26:40 2026 \u002B0200",
        "message": "chore: ship the v3.0.0 API (#1779)"
      },
      {
        "sha": "fa595d4790a49f64d5cf9e958b216ba40f17c5ad",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:27:33 2026 \u002B0200",
        "message": "docs: lead the \u0022Why aweXpect?\u0022 tabs with the extension packages (#1780)"
      },
      {
        "sha": "5cf6a80d0462504a666c7b204cded9d12cec0512",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 08:48:31 2026 \u002B0200",
        "message": "docs: show a consumer-side \u0060AndIf\u0060 for expectations that only apply under a condition (#1781)"
      },
      {
        "sha": "29e1127b443bc230edabceb08a728d9198753e20",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:26:33 2026 \u002B0200",
        "message": "perf: skip the repeated check in \u0060Satisfies\u0060 and \u0060CompliesWith\u0060 without a timeout (#1782)"
      },
      {
        "sha": "779048624672829223d4e1de669d23b41bf06597",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:39:04 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1783)"
      },
      {
        "sha": "c9eb2cefda18397f58824b92a06bbcd9b8261914",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:51:34 2026 \u002B0200",
        "message": "perf: skip the failure texts of a met \u0060InAnyOrder().IgnoringDuplicates()\u0060 (#1789)"
      },
      {
        "sha": "79d6813aeaac4319dfdcb670e428b84b6bd5693e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:52:06 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1790)"
      },
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "647847aa",
      "272bbb23",
      "23b6cd56",
      "a1b234c6",
      "168dc965",
      "f21e6eab",
      "d98a48a7",
      "0c874997",
      "0d6d920b",
      "c8e40f00",
      "a8e6cbb0",
      "82b90d2e",
      "e0c39d6f",
      "3a0d86a4",
      "e4877b71",
      "e4e4a0e6",
      "9f96f8b9",
      "a07a99e2",
      "998fbcba",
      "464696d1",
      "1f967b58",
      "7e4f4bdb",
      "93d22f80",
      "8b666ac8",
      "8c8f6bf6",
      "6242c56e",
      "f7a0873f",
      "8e2da4f3",
      "41cb59cc",
      "75cde8b0",
      "3fd1189b",
      "fe613c6c",
      "5ef58e1e",
      "28acb579",
      "75406bf0",
      "fa595d47",
      "5cf6a80d",
      "29e1127b",
      "77904862",
      "c9eb2cef",
      "79d6813a",
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          706.9937650680542,
          716.2311901728312,
          746.4471676690238,
          586.6084758894784,
          774.0639625276838,
          365.76004629135133,
          355.55527383940563,
          744.1354974110922,
          765.0326632772174,
          774.3237487792969,
          815.4775744756063,
          735.5506409327189,
          719.7604413986206,
          671.5482345581055,
          664.6221086638315,
          726.6206457773844,
          826.8584680557251,
          370.2186816419874,
          720.0846664428711,
          720.6847084681193,
          577.747324625651,
          353.08312681743075,
          343.5415005683899,
          737.9813050490159,
          363.8018371141874,
          372.7434025491987,
          717.8476252555847,
          720.1078741709391,
          725.8311898549398,
          851.6755803426107,
          765.237514087132,
          783.9828212102254,
          556.5622659410749,
          722.856612141927,
          521.4267307428213,
          369.14770089662994,
          712.0842841466268,
          759.7327707926432,
          787.5936194828579,
          713.5918260301862,
          390.10694820540294,
          717.9856726328532,
          793.373016016824,
          728.0096852620443,
          677.8588190759931,
          402.8931168556213,
          711.3378532273429,
          561.8075871149699,
          445.76871263980865,
          744.3005462793203
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
          1808,
          1808,
          1808,
          1808,
          1808,
          1808,
          1808,
          1808,
          1808,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1824,
          1824,
          1824,
          1824,
          1824,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1840,
          1848,
          1848
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
          81257.22533307757,
          81844.91207682292,
          82125.86353410993,
          53291.844130452475,
          84826.06088867187,
          28388.506831577844,
          28697.135393415178,
          81594.28129069011,
          81419.26825823102,
          85156.56563626803,
          85032.92860630581,
          83092.55021972656,
          79968.29377629206,
          59742.699979341945,
          30550.12803867885,
          80930.81984165737,
          55566.31945800781,
          28806.078369140625,
          59803.41401890346,
          61225.11710030692,
          53141.57476399739,
          29587.716825212752,
          30830.735575358074,
          62886.03064778646,
          28914.269256591797,
          28976.523685709635,
          82400.20458170572,
          83982.37623697917,
          82909.91080147879,
          84981.80950055804,
          83312.36499023438,
          84426.20920410156,
          52025.71477801983,
          82344.41915189303,
          50964.05476888021,
          27934.408272879464,
          75639.69289456881,
          86196.58836669922,
          82570.19873046875,
          32312.39946637835,
          29732.30352376302,
          82956.58256835937,
          85194.58972981772,
          34076.88885498047,
          59926.632983398435,
          32037.00215657552,
          83777.21427408855,
          51433.28704364483,
          31058.216958852914,
          82880.52755972055
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
          5252,
          5252,
          5252,
          5256,
          5252,
          5256,
          5256,
          5252,
          5252,
          5252,
          5252,
          5252,
          5252,
          5252,
          5256,
          5252,
          5247,
          5256,
          5252,
          5252,
          5256,
          5256,
          5256,
          5254,
          5256,
          5256,
          5252,
          5252,
          5252,
          5252,
          5252,
          5252,
          5256,
          5252,
          5256,
          5256,
          5252,
          5252,
          5252,
          5256,
          5256,
          5252,
          5252,
          5256,
          5252,
          5256,
          5252,
          5256,
          5256,
          5252
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
  "Satisfies": {
    "commits": [
      {
        "sha": "647847aae6bff0b87edd4fb2b0f31ea389537e57",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:31 2026 \u002B0200",
        "message": "fix: enumerate the expected sequence of collection \u0060IsEqualTo\u0060 and \u0060IsContainedIn\u0060 only once per expectation (#1668)"
      },
      {
        "sha": "272bbb23db0b17bf4bfdffd1a5431707dacffa73",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:14:59 2026 \u002B0200",
        "message": "fix: skip every type that is compared by value or by content in the metadata generator (#1667)"
      },
      {
        "sha": "23b6cd56dbd45d5be7a56d4d2fc91ba0f1b6fba1",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:15:43 2026 \u002B0200",
        "message": "fix: accept finite numbers whose distance overflows in strict ordering with an infinite tolerance (#1666)"
      },
      {
        "sha": "a1b234c6becec264c0e5affca08319172f105a7c",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 19:16:17 2026 \u002B0200",
        "message": "fix!: do not offer a second \u0060Matching\u0060 on the result of \u0060HasSingle().Matching\u2026\u0060 (#1663)"
      },
      {
        "sha": "168dc965e6b1c84d256ac40eb02d77d52b142043",
        "author": "Valentin Breu\u00DF",
        "date": "Mon Oct 5 22:13:55 2026 \u002B0200",
        "message": "fix!: reject a property expression of \u0060TriggeredPropertyChangedFor\u0060 that is no property of the subject (#1665)"
      },
      {
        "sha": "f21e6eaba602e779f0efe0ae83e392121e673f0e",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 06:36:57 2026 \u002B0200",
        "message": "refactor: make the timeout and retry tests independent of the machine speed (#1658)"
      },
      {
        "sha": "d98a48a7c9027a5e9a3fb8092b2b7eb259848f11",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 07:20:25 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.16 (#1670)"
      },
      {
        "sha": "0c874997620abfae9aec2928a18f9bffbd2ee840",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 08:34:19 2026 \u002B0200",
        "message": "refactor: reduce the cognitive complexity of \u0060EmitRegistration\u0060 in the \u0060TypeMetadataGenerator\u0060 (#1671)"
      },
      {
        "sha": "0d6d920b6328f9cb9a22fe6e17b1c17be8dd5cfa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 6 18:34:38 2026 \u002B0200",
        "message": "test: migrate the unit tests from xUnit v2 to TUnit (#1672)"
      },
      {
        "sha": "c8e40f0059cc58502f272fc46bb6c8133187ecb3",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:07:05 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.17 (#1687)"
      },
      {
        "sha": "a8e6cbb0921667eb9773fa25913c69b0c4430849",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:17:05 2026 \u002B0200",
        "message": "test: make real-time dependent tests of \u0060Eventually\u0060, \u0060Polling\u0060 and the signaler deterministic (#1686)"
      },
      {
        "sha": "82b90d2ea7e6b0152df0739bbbee5fdaada69a39",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 12:50:55 2026 \u002B0200",
        "message": "ci: identify the pull request of an analysed run by the event instead of its artifacts (#1688)"
      },
      {
        "sha": "e0c39d6f3fd9d409274b60c5a932bada9f55a9c0",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:40:10 2026 \u002B0200",
        "message": "fix: describe the last attempt in \u0060IsOneOf\u0060 and \u0060IsNotOneOf\u0060 on \u0060DateTime\u0060 after an earlier incompatible kind (#1689)"
      },
      {
        "sha": "3a0d86a45c60afc2bdef37100570c1e1ac13bddc",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 13:53:09 2026 \u002B0200",
        "message": "ci: disable the mutation tests and pin the Stryker version (#1693)"
      },
      {
        "sha": "e4877b71814748aeb8eae0bfbd3698446d51a28a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:11:36 2026 \u002B0200",
        "message": "test: pin the NUnit 4 adapter tests to NUnit 4 and add a project for NUnit 5 (#1690)"
      },
      {
        "sha": "e4e4a0e6d5366735cd810bd521c7f2cc8e8202c1",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:39:49 2026 \u002B0200",
        "message": "fix!: hand out a non-null subject when awaiting \u0060IsParsableInto\u0060 on a string (#1691)"
      },
      {
        "sha": "9f96f8b98b0347930a55827ba71f5ac3a27432eb",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 14:42:31 2026 \u002B0200",
        "message": "ci: close gaps in the test runs and the public API check of pull requests (#1692)"
      },
      {
        "sha": "a07a99e21c62fcc44d71c973dc607c32b262670a",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 15:25:38 2026 \u002B0200",
        "message": "fix: bound the check of an attempt of \u0060Eventually()\u0060 by the retry budget (#1695)"
      },
      {
        "sha": "998fbcba07f55588650d37e481c620b6925b626d",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:09:27 2026 \u002B0200",
        "message": "fix: fail the expectation when the enumerator or the current item of a materialized subject throws (#1697)"
      },
      {
        "sha": "464696d177a9f9c12b445c4c827f1ac2a073c091",
        "author": "Valentin Breu\u00DF",
        "date": "Wed Oct 7 16:11:22 2026 \u002B0200",
        "message": "fix: keep the outcome of a quantified expectation when user code throws after the deciding item of a collection with a known count (#1698)"
      },
      {
        "sha": "1f967b5891c9d2474c74e7122df47418ba35c8c0",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 12:19:03 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.18 (#1740)"
      },
      {
        "sha": "7e4f4bdba98cfa1eddad59a18b83cbdf776c174d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:14:00 2026 \u002B0200",
        "message": "fix: show the whole string difference when the minimum number of characters after it is close to \u0060int.MaxValue\u0060 (#1741)"
      },
      {
        "sha": "93d22f80c1c71b9006b4950886cc204fc899957c",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:23:23 2026 \u002B0200",
        "message": "fix: do not suppress nullability warnings when the statement of the expectation overwrites the subject (#1742)"
      },
      {
        "sha": "8b666ac8f8b63072a96fd3152182e03b2cd192f8",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:31:43 2026 \u002B0200",
        "message": "fix!: check the kinds of \u0060DateTime\u0060 values in an untyped collection when verifying its order (#1744)"
      },
      {
        "sha": "8c8f6bf6b6495d38ccf74cdd4c25ae541f0b3e6d",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:36:07 2026 \u002B0200",
        "message": "fix: keep an event recording running across the repeated checks of \u0060CompliesWith(\u2026).Within(\u2026)\u0060 (#1745)"
      },
      {
        "sha": "6242c56e2ef28022bdbd22a141f2dd24a3d30374",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 15:50:50 2026 \u002B0200",
        "message": "fix!: pass a null argument to the predicate of the positional event filters (#1747)"
      },
      {
        "sha": "f7a0873ff8b24ee55e1f59c3990cf1ab57f61be6",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 16:06:38 2026 \u002B0200",
        "message": "fix: stop reading the source for the failure message of the synchronous \u0060IsEmpty\u0060 (#1746)"
      },
      {
        "sha": "8e2da4f371eda53d393d9e80ba7a07a78b9b2905",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:01:52 2026 \u002B0200",
        "message": "chore: bump aweXpect to v3.0.0-pre.19 (#1753)"
      },
      {
        "sha": "41cb59cca90ed6a485f0c71418670b19988dc75b",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 19:34:01 2026 \u002B0200",
        "message": "docs: shorten the v3 migration guide and trim the reference pages (#1754)"
      },
      {
        "sha": "75cde8b0fa6fde9400552cb23a296a087933c892",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:11:19 2026 \u002B0200",
        "message": "chore: publish to nuget.org via trusted publishing (#1756)"
      },
      {
        "sha": "3fd1189bbcc7dcb05cc18646d823f18924ce7e17",
        "author": "Valentin Breu\u00DF",
        "date": "Thu Oct 8 21:17:53 2026 \u002B0200",
        "message": "feat: add subject contexts to the expectation builder (#1755)"
      },
      {
        "sha": "fe613c6c5bb8baf0e41ddad6c7f2af5805bbdc64",
        "author": "Valentin Breu\u00DF",
        "date": "Fri Oct 9 06:15:39 2026 \u002B0200",
        "message": "docs: address review feedback on configuration, date/time and equivalency pages (#1757)"
      },
      {
        "sha": "5ef58e1ecd62ea196c69ed0946ef275e6cf8454e",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 08:39:58 2026 \u002B0200",
        "message": "chore: bump aweXpect.Core to v3.0.0 (#1777)"
      },
      {
        "sha": "28acb579dcb9a53f2fdd4cb58dbbf5dce70d4d5f",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 10:34:33 2026 \u002B0200",
        "message": "docs: clarify the once-only rule for extension options and past-tense naming for recorded occurrences (#1778)"
      },
      {
        "sha": "75406bf0523ab761f42b1ce387c81eede7daf1e6",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:26:40 2026 \u002B0200",
        "message": "chore: ship the v3.0.0 API (#1779)"
      },
      {
        "sha": "fa595d4790a49f64d5cf9e958b216ba40f17c5ad",
        "author": "Valentin Breu\u00DF",
        "date": "Sat Oct 10 12:27:33 2026 \u002B0200",
        "message": "docs: lead the \u0022Why aweXpect?\u0022 tabs with the extension packages (#1780)"
      },
      {
        "sha": "5cf6a80d0462504a666c7b204cded9d12cec0512",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 08:48:31 2026 \u002B0200",
        "message": "docs: show a consumer-side \u0060AndIf\u0060 for expectations that only apply under a condition (#1781)"
      },
      {
        "sha": "29e1127b443bc230edabceb08a728d9198753e20",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:26:33 2026 \u002B0200",
        "message": "perf: skip the repeated check in \u0060Satisfies\u0060 and \u0060CompliesWith\u0060 without a timeout (#1782)"
      },
      {
        "sha": "779048624672829223d4e1de669d23b41bf06597",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 09:39:04 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1783)"
      },
      {
        "sha": "c9eb2cefda18397f58824b92a06bbcd9b8261914",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:51:34 2026 \u002B0200",
        "message": "perf: skip the failure texts of a met \u0060InAnyOrder().IgnoringDuplicates()\u0060 (#1789)"
      },
      {
        "sha": "79d6813aeaac4319dfdcb670e428b84b6bd5693e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 10:52:06 2026 \u002B0200",
        "message": "perf: speed up \u0060InAnyOrder()\u0060 for collections of equal items (#1790)"
      },
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "647847aa",
      "272bbb23",
      "23b6cd56",
      "a1b234c6",
      "168dc965",
      "f21e6eab",
      "d98a48a7",
      "0c874997",
      "0d6d920b",
      "c8e40f00",
      "a8e6cbb0",
      "82b90d2e",
      "e0c39d6f",
      "3a0d86a4",
      "e4877b71",
      "e4e4a0e6",
      "9f96f8b9",
      "a07a99e2",
      "998fbcba",
      "464696d1",
      "1f967b58",
      "7e4f4bdb",
      "93d22f80",
      "8b666ac8",
      "8c8f6bf6",
      "6242c56e",
      "f7a0873f",
      "8e2da4f3",
      "41cb59cc",
      "75cde8b0",
      "3fd1189b",
      "fe613c6c",
      "5ef58e1e",
      "28acb579",
      "75406bf0",
      "fa595d47",
      "5cf6a80d",
      "29e1127b",
      "77904862",
      "c9eb2cef",
      "79d6813a",
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          277.62686608632407,
          262.4779659679958,
          303.72475092751637,
          225.3275263150533,
          303.956152788798,
          135.53794468366183,
          136.48703560462366,
          283.5047616958618,
          291.08639560426985,
          239.75034195582072,
          304.97349578993663,
          246.66691926320394,
          232.86030954974038,
          238.89018327849251,
          247.55775231581467,
          240.59601265589396,
          283.48961407343546,
          126.06770662466685,
          256.43952117647444,
          233.4618682463964,
          196.59285897475021,
          124.18757068193875,
          140.5408843755722,
          259.0540941874186,
          125.38672533035279,
          125.47152725287846,
          243.43688027064005,
          266.39192117055256,
          265.87161350250244,
          259.8366877237956,
          275.5710287412008,
          256.67651211420696,
          192.95787699405963,
          266.3875561396281,
          193.56110445658365,
          127.1776643594106,
          216.63132432790903,
          202.07457439104715,
          182.88593041102092,
          200.87237639427184,
          104.90384703023093,
          195.21221709251404,
          186.61623595555622,
          209.69154957135518,
          179.5712014266423,
          119.93977559529819,
          178.88042221750533,
          156.0302222887675,
          120.64632336298625,
          183.5748676141103
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
          656,
          656,
          656,
          656,
          656,
          656,
          656,
          656,
          656,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          664,
          672,
          672,
          672,
          672,
          672,
          688,
          688,
          688,
          688,
          688,
          584,
          584,
          584,
          584,
          584,
          584,
          584,
          584,
          584,
          584,
          584,
          552,
          552
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
          30893.274315467246,
          31673.233797200523,
          32372.2741007487,
          15254.764650785006,
          30786.992837088448,
          9273.870933532715,
          9122.359176635742,
          30918.375451660155,
          30489.133728027344,
          31956.172403971355,
          33126.5643758138,
          30889.731115722658,
          30972.008573091945,
          18302.631251408504,
          16101.794182332356,
          30843.401501464843,
          26076.549479166668,
          10431.874232365535,
          18769.713187081474,
          18734.728539603097,
          14834.462311808269,
          9697.194235229492,
          10682.660048421223,
          24303.289461263023,
          10047.785783894857,
          9949.456902640206,
          30220.73620402018,
          32154.62091878255,
          31809.45400390625,
          32316.998864746092,
          30731.354388897234,
          31265.845668247766,
          15174.909043532152,
          31606.306352887834,
          15129.55615234375,
          9928.43082784017,
          30163.22892543248,
          32356.10744628906,
          32051.593680826823,
          16588.627480643136,
          10516.453231811523,
          32024.219323730467,
          32504.827705891927,
          16406.975264485678,
          18368.450517926896,
          10934.848035539899,
          30092.941772460938,
          15035.3517964681,
          12654.644130452474,
          32423.43391113281
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
          5614,
          5614,
          5614,
          5615,
          5614,
          5615,
          5615,
          5614,
          5614,
          5614,
          5614,
          5614,
          5614,
          5614,
          5614,
          5614,
          5615,
          5615,
          5614,
          5614,
          5615,
          5615,
          5615,
          5615,
          5615,
          5615,
          5614,
          5614,
          5614,
          5614,
          5614,
          5614,
          5615,
          5614,
          5615,
          5615,
          5614,
          5614,
          5614,
          5614,
          5615,
          5614,
          5614,
          5614,
          5614,
          5615,
          5614,
          5614,
          5614,
          5614
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          242.6730498313904,
          236.24986244837444,
          249.67201636632282,
          197.15859258969624,
          259.8181400934855,
          119.45650717417399,
          124.34598207473755,
          249.06301546096802,
          239.74138558705647,
          260.66469326019285,
          281.2242929385259,
          254.91282835006714,
          234.76506390571595,
          228.9157928029696,
          218.87901401519775,
          233.129546216556,
          264.4644939740499,
          133.89500074386598,
          245.34382966586523,
          246.47795225779217,
          181.4882709639413,
          122.21910122462681,
          146.55186425722562,
          245.19413394927977,
          131.45483563496515,
          124.23931743303935,
          246.91170746485392,
          243.68746512730917,
          246.58337157567343,
          258.87168684005735,
          248.2426520415715,
          244.78331689834596,
          181.11447252546037,
          243.9997957433973,
          183.17132539749144,
          119.09006812022282,
          219.57049543062845,
          245.98374554316203,
          239.95667231877644,
          237.77133469922202,
          128.79864185651144,
          245.19918336868287,
          250.1819934209188,
          238.0412252744039,
          235.80043573379515,
          133.22090535504478,
          240.8700272242228,
          183.7677354812622,
          147.67371101379393,
          255.7457534154256
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712,
          712
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "Contains": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          619.0683628082276,
          646.3657181603568,
          511.58852250235424,
          600.5068825208224,
          327.77489624704634,
          611.9652239935739,
          484.3638657251994,
          335.4380095163981,
          619.9793544769287
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
          728,
          728,
          728,
          728,
          728,
          728,
          728,
          696,
          696
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
          376.7006852149963,
          389.9646262804667,
          462.31690947214764,
          372.8653973511287,
          227.01778138478596,
          357.3124629020691,
          307.95345500310265,
          263.01649208068847,
          373.8811431271689
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
          1608,
          1608,
          1608,
          1608,
          1608,
          1608,
          1608,
          1608,
          1608
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          404.0439930597941,
          419.9353549003601,
          392.52065215791976,
          417.34871571858724,
          226.67296517690022,
          404.22499952316286,
          345.89806168874105,
          264.56047274271646,
          398.0087053934733
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          848,
          848,
          848,
          848,
          848,
          848,
          848,
          848,
          848
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "Multiple": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1027.7238175710042,
          1074.380944115775,
          1017.3066056569418,
          1021.8631283686711,
          572.6790697915213,
          1031.8272314071655,
          872.7421139399211,
          640.7282141367594,
          1151.8307791392008
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
          2168,
          2168,
          2168,
          2168,
          2168,
          2168,
          2168,
          1720,
          1720
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
          1915.7398962974548,
          1914.2061210632323,
          2152.189163208008,
          1830.9006284077961,
          1129.5604832512993,
          1873.955077234904,
          1627.4117516108922,
          1279.0739683004526,
          1887.3938435872396
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
          6744,
          6744,
          6744,
          6744,
          6744,
          6744,
          6744,
          6744,
          6744
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          956.1783354622977,
          916.2488430023193,
          932.6948055903117,
          869.4494672921987,
          527.6286060626691,
          886.0307658513387,
          795.0054859161377,
          584.2220613039457,
          949.4310310999552
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          2240,
          2240,
          2240,
          2240,
          2240,
          2240,
          2240,
          2240,
          2240
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "Throws": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          13593.631301879883,
          13820.319644383022,
          11157.852619425455,
          12161.19014798678,
          7416.879830932618,
          13758.310784476143,
          9684.503852844238,
          7080.700775733361,
          12135.059914725167
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
          2360,
          2360,
          2360,
          2360,
          2360,
          2360,
          2360,
          1720,
          1720
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
          42531.71004231771,
          42031.99049886068,
          32374.797349196215,
          33724.59740702311,
          19892.586547851562,
          41125.15801595052,
          27674.64548746745,
          23994.497277396065,
          42559.10313197545
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
          18609,
          18609,
          18609,
          18609,
          18609,
          18609,
          18609,
          18608,
          18609
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          11259.778454120342,
          11182.145147470328,
          9627.469880240304,
          10518.905581883022,
          6714.693571472168,
          11382.084618201623,
          8296.059771024264,
          6847.346667698452,
          11270.076776776996
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          1824,
          1824,
          1824,
          1824,
          1824,
          1824,
          1824,
          1824,
          1824
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      }
    ]
  },
  "ThrowsAsync": {
    "commits": [
      {
        "sha": "16e103dc8f5e9a0c17a8a06a35bc288aec1cc782",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:21:19 2026 \u002B0200",
        "message": "docs: rework the benchmarks for a fairer and broader comparison (#1793)"
      },
      {
        "sha": "68b48fefc5c46acee092fa25e5a7fa916dc6b0f2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:24:29 2026 \u002B0200",
        "message": "perf: keep only the items that a quantifier can show (#1784)"
      },
      {
        "sha": "bb7944b0365419194033db0e4973e369cf495d99",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:31:23 2026 \u002B0200",
        "message": "perf: compare numbers without boxing in \u0060IsEqualTo\u0060 (#1786)"
      },
      {
        "sha": "f81fa443ff220fae1d991babf7e02b45a43f965e",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:34:23 2026 \u002B0200",
        "message": "perf: trim the subject expression only when it is read (#1787)"
      },
      {
        "sha": "d62d702f58ec0ecb8ff99a27a32cf5cf58225ce9",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:52:28 2026 \u002B0200",
        "message": "perf: share parsed regexes between expectations (#1788)"
      },
      {
        "sha": "d79dbb79baa8b3b5b170c885e4ad77ee368fcb41",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 11:53:02 2026 \u002B0200",
        "message": "perf: keep an \u0060Action\u0060 or \u0060Func\u003CTask\u003E\u0060 subject without a wrapping closure (#1785)"
      },
      {
        "sha": "ad09d2cac018e26274c219bd0d2a03fbcf3809a0",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 12:04:03 2026 \u002B0200",
        "message": "perf: add \u0060Which\u0060 continuations without closures (#1791)"
      },
      {
        "sha": "25bdf4e4b7ed0a031fc3873065925fe191f8a815",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 14:49:19 2026 \u002B0200",
        "message": "chore: count the items of an array in the items count benchmark (#1810)"
      },
      {
        "sha": "bea504d55f003736ca7abde9ae6f66d9150549c2",
        "author": "Valentin Breu\u00DF",
        "date": "Sun Oct 11 15:05:37 2026 \u002B0200",
        "message": "perf: let a consumer read a materialized sequence by position (#1809)"
      }
    ],
    "labels": [
      "16e103dc",
      "68b48fef",
      "bb7944b0",
      "f81fa443",
      "d62d702f",
      "d79dbb79",
      "ad09d2ca",
      "25bdf4e4",
      "bea504d5"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          23877.85376194545,
          24259.805905855617,
          19651.033197021483,
          21156.631940569197,
          12929.774574788411,
          23753.03332737514,
          16683.66478553185,
          14552.560348510742,
          23663.978812081474
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
          3064,
          3064,
          3064,
          3064,
          3064,
          3064,
          3064,
          2640,
          2640
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
          62038.14596354167,
          61007.11339518229,
          46709.029811314176,
          48864.6248953683,
          28692.68379865374,
          60304.13727678572,
          39463.85021972656,
          33270.43501281738,
          61104.8171648298
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
          23554,
          23554,
          23553,
          23553,
          23553,
          23553,
          23553,
          23553,
          23554
        ],
        "borderColor": "#FF671B",
        "backgroundColor": "#FF671B",
        "yAxisID": "y1",
        "borderDash": [
          5,
          5
        ],
        "pointStyle": "triangle"
      },
      {
        "label": "TUnit time",
        "unit": "ns",
        "data": [
          22360.791755089394,
          22230.03127179827,
          18932.40057373047,
          20613.838934762138,
          13156.13920694987,
          22183.806962076822,
          16107.392168317523,
          13545.327889578683,
          22388.99493408203
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
        "yAxisID": "y",
        "borderDash": [],
        "pointStyle": "circle"
      },
      {
        "label": "TUnit memory",
        "unit": "b",
        "data": [
          2920,
          2920,
          2920,
          2920,
          2920,
          2920,
          2920,
          2920,
          2920
        ],
        "borderColor": "#1A6029",
        "backgroundColor": "#1A6029",
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