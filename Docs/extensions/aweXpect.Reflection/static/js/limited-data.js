window.BENCHMARK_DATA = {
  "TypeHasAttribute": {
    "commits": [
      {
        "sha": "68feb62f7303b2dcab4f3972ec6d54f05e5a4e62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:45:23 2026 \u002B0200",
        "message": "feat!: release all aweXpect packages from one repository with one version"
      },
      {
        "sha": "66de07e4365ee3790661f33baba3106193b6e939",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:48:15 2026 \u002B0200",
        "message": "chore: rename the ReSharper settings back to aweXpect.sln.DotSettings (#1887)"
      },
      {
        "sha": "391462ba40ae1a1a36545de23ad94db20b75d605",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 13:59:10 2026 \u002B0200",
        "message": "fix: do not deadlock at the synchronous boundaries of Reflection and Snapshots (#1889)"
      },
      {
        "sha": "78b13d35945c33c3d1419aee1d8b7dceea1e08c5",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:00:45 2026 \u002B0200",
        "message": "refactor: speed up matching JSON against an expected object (#1891)"
      },
      {
        "sha": "87d3aeb20d199c102d1f4c48342c1512bbedc655",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:03:06 2026 \u002B0200",
        "message": "refactor: format the expected status code only when the expectation fails (#1894)"
      },
      {
        "sha": "fe413769d7b2d5c371fae9dae4e8394586b5c644",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:04:53 2026 \u002B0200",
        "message": "refactor: speed up architecture rules on the types of an assembly (#1893)"
      },
      {
        "sha": "3514e5d3bfab44d5333713f720a8f883c8e4bdab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:09:35 2026 \u002B0200",
        "message": "refactor: make subject contexts, member expectations and materialized collections cheaper on the happy path (#1895)"
      },
      {
        "sha": "dd7508bde0e65a3e19ef89c6d72b1b68156791b1",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:37:26 2026 \u002B0200",
        "message": "refactor: compare XML and look up XML nodes faster on the happy path (#1892)"
      }
    ],
    "labels": [
      "68feb62f",
      "66de07e4",
      "391462ba",
      "78b13d35",
      "87d3aeb2",
      "fe413769",
      "3514e5d3",
      "dd7508bd"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          725.2527442932129,
          740.71807568868,
          1173.8407468795776,
          1204.02774365743,
          1150.5083653376653,
          1470.658320681254,
          1445.271382522583,
          1177.0453611101423
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
          992,
          992,
          992,
          992,
          992,
          992,
          992,
          992
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
          511.2527196747916,
          621.1502604484558,
          924.3166027069092,
          959.6624974470872,
          854.7111960093181,
          898.764992918287,
          897.090498311179,
          893.2992651121957
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
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1248,
          1248
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
  "TypeIsNotStatic": {
    "commits": [
      {
        "sha": "68feb62f7303b2dcab4f3972ec6d54f05e5a4e62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:45:23 2026 \u002B0200",
        "message": "feat!: release all aweXpect packages from one repository with one version"
      },
      {
        "sha": "66de07e4365ee3790661f33baba3106193b6e939",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:48:15 2026 \u002B0200",
        "message": "chore: rename the ReSharper settings back to aweXpect.sln.DotSettings (#1887)"
      },
      {
        "sha": "391462ba40ae1a1a36545de23ad94db20b75d605",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 13:59:10 2026 \u002B0200",
        "message": "fix: do not deadlock at the synchronous boundaries of Reflection and Snapshots (#1889)"
      },
      {
        "sha": "78b13d35945c33c3d1419aee1d8b7dceea1e08c5",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:00:45 2026 \u002B0200",
        "message": "refactor: speed up matching JSON against an expected object (#1891)"
      },
      {
        "sha": "87d3aeb20d199c102d1f4c48342c1512bbedc655",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:03:06 2026 \u002B0200",
        "message": "refactor: format the expected status code only when the expectation fails (#1894)"
      },
      {
        "sha": "fe413769d7b2d5c371fae9dae4e8394586b5c644",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:04:53 2026 \u002B0200",
        "message": "refactor: speed up architecture rules on the types of an assembly (#1893)"
      },
      {
        "sha": "3514e5d3bfab44d5333713f720a8f883c8e4bdab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:09:35 2026 \u002B0200",
        "message": "refactor: make subject contexts, member expectations and materialized collections cheaper on the happy path (#1895)"
      },
      {
        "sha": "dd7508bde0e65a3e19ef89c6d72b1b68156791b1",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:37:26 2026 \u002B0200",
        "message": "refactor: compare XML and look up XML nodes faster on the happy path (#1892)"
      }
    ],
    "labels": [
      "68feb62f",
      "66de07e4",
      "391462ba",
      "78b13d35",
      "87d3aeb2",
      "fe413769",
      "3514e5d3",
      "dd7508bd"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          74.25871433661534,
          111.08356503077916,
          151.24119797774725,
          156.74714631693703,
          140.93855563231878,
          141.24543137550353,
          144.55080912907917,
          128.96873346396856
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
          384,
          384,
          384,
          384,
          384,
          384,
          384,
          384
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
          124.19380749974933,
          235.07810057004292,
          245.9582951227824,
          255.5991012573242,
          200.72816676752907,
          215.40178451538085,
          241.9134787241618,
          197.14040073981653
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
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032,
          1032
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
  "TypesAreSealed": {
    "commits": [
      {
        "sha": "68feb62f7303b2dcab4f3972ec6d54f05e5a4e62",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:45:23 2026 \u002B0200",
        "message": "feat!: release all aweXpect packages from one repository with one version"
      },
      {
        "sha": "66de07e4365ee3790661f33baba3106193b6e939",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 09:48:15 2026 \u002B0200",
        "message": "chore: rename the ReSharper settings back to aweXpect.sln.DotSettings (#1887)"
      },
      {
        "sha": "391462ba40ae1a1a36545de23ad94db20b75d605",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 13:59:10 2026 \u002B0200",
        "message": "fix: do not deadlock at the synchronous boundaries of Reflection and Snapshots (#1889)"
      },
      {
        "sha": "78b13d35945c33c3d1419aee1d8b7dceea1e08c5",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:00:45 2026 \u002B0200",
        "message": "refactor: speed up matching JSON against an expected object (#1891)"
      },
      {
        "sha": "87d3aeb20d199c102d1f4c48342c1512bbedc655",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:03:06 2026 \u002B0200",
        "message": "refactor: format the expected status code only when the expectation fails (#1894)"
      },
      {
        "sha": "fe413769d7b2d5c371fae9dae4e8394586b5c644",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:04:53 2026 \u002B0200",
        "message": "refactor: speed up architecture rules on the types of an assembly (#1893)"
      },
      {
        "sha": "3514e5d3bfab44d5333713f720a8f883c8e4bdab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:09:35 2026 \u002B0200",
        "message": "refactor: make subject contexts, member expectations and materialized collections cheaper on the happy path (#1895)"
      },
      {
        "sha": "dd7508bde0e65a3e19ef89c6d72b1b68156791b1",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 14:37:26 2026 \u002B0200",
        "message": "refactor: compare XML and look up XML nodes faster on the happy path (#1892)"
      }
    ],
    "labels": [
      "68feb62f",
      "66de07e4",
      "391462ba",
      "78b13d35",
      "87d3aeb2",
      "fe413769",
      "3514e5d3",
      "dd7508bd"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          8639.393259684244,
          20809.158352661132,
          20136.952707926434,
          20580.747135416666,
          21500.484102376304,
          2299.7091654459637,
          1979.3088559468588,
          1956.0963940938313
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
          6768,
          6770,
          6793,
          6783,
          6781,
          2784,
          2376,
          2376
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
          654.906969944636,
          919.966920705942,
          1323.509884561811,
          1246.4895590373449,
          1191.416109085083,
          1231.1467041015626,
          1247.7940812792096,
          1228.7658562978108
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
          1760,
          1760,
          1760,
          1760,
          1760,
          1760,
          1760,
          1760
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