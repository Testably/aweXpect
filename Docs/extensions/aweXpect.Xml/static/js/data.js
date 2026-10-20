window.BENCHMARK_DATA = {
  "Attribute": {
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
      },
      {
        "sha": "39b404f73af872eff13dce43f4ae86fc675cbe63",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 16:03:18 2026 \u002B0200",
        "message": "docs: update the README and the extension packages overview (#1897)"
      },
      {
        "sha": "b5dab01f19c9c2a9669e9f2ea53356ffa9349652",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 16:36:22 2026 \u002B0200",
        "message": "refactor: format values without allocating a context and a builder when a custom formatter is registered (#1896)"
      },
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
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
      "dd7508bd",
      "39b404f7",
      "b5dab01f",
      "40ae946f"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1097.8751047679357,
          1078.875713857015,
          806.6300570624215,
          915.2430147024302,
          1148.4123452504475,
          1153.9634455362955,
          1014.881681060791,
          900.8595012937274,
          900.7302189509074,
          865.0074442454746,
          698.0744481767927
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
          1816,
          1816,
          1816,
          1816,
          1816,
          1816,
          1488,
          1312,
          1312,
          1312,
          1312
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
          730.8021795409066,
          668.750609261649,
          528.2589103062948,
          596.14315478007,
          780.3684758458819,
          781.9141397476196,
          727.8090354283651,
          751.0237486179059,
          682.8371315638225,
          723.0871920267741,
          674.2990703582764
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
          2776,
          2776,
          2776,
          2776,
          2776,
          2776,
          2776,
          2776,
          2776,
          2776,
          2776
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
  "XmlString": {
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
      },
      {
        "sha": "39b404f73af872eff13dce43f4ae86fc675cbe63",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 16:03:18 2026 \u002B0200",
        "message": "docs: update the README and the extension packages overview (#1897)"
      },
      {
        "sha": "b5dab01f19c9c2a9669e9f2ea53356ffa9349652",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 16:36:22 2026 \u002B0200",
        "message": "refactor: format values without allocating a context and a builder when a custom formatter is registered (#1896)"
      },
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
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
      "dd7508bd",
      "39b404f7",
      "b5dab01f",
      "40ae946f"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          12670.111607142857,
          12171.387325032552,
          8267.12919834682,
          9192.517417399089,
          13008.33838704427,
          13387.325060163226,
          11934.761827741351,
          5255.899936676025,
          4890.580227661133,
          4768.738585408529,
          4462.017372131348
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
          34880,
          34885,
          34885,
          34885,
          34880,
          34885,
          34661,
          24487,
          24487,
          24487,
          24487
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
          5336.80074603741,
          5097.102923583984,
          3744.885112489973,
          4400.290140005259,
          6438.485093688965,
          6425.82346089681,
          5026.074538094656,
          5954.222114562988,
          5368.210540262858,
          5186.283226776123,
          4998.080684026082
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
          25758,
          25758,
          25758,
          25758,
          25758,
          25758,
          25758,
          25758,
          25758,
          25758,
          25758
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
  "ElementMatches": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      }
    ],
    "labels": [
      "40ae946f"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          2657.705650874547
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
          13032
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
          3306.319850921631
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
          14443
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
  "ElementOrder": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      }
    ],
    "labels": [
      "40ae946f"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          37174.54638671875
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
          99704
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
          222737.04000418526
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
          544246
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