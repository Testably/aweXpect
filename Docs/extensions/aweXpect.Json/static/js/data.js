window.BENCHMARK_DATA = {
  "JsonString": {
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
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
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
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          43654.32958984375,
          44267.41297325721,
          42254.887282151445,
          2442.302179609026,
          1307.5368900299072,
          1929.9819597516741,
          2074.014804295131,
          2446.018781389509,
          2389.7055789402552,
          2357.9106992085776,
          2373.6216667175295,
          1397.9846696217855
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
          14531,
          14541,
          14530,
          2520,
          2520,
          2520,
          2520,
          2520,
          2520,
          2520,
          2520,
          1344
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
          16047.227347237724,
          16217.150813176082,
          14543.159549419697,
          14057.160717303936,
          6657.376680227427,
          10324.007733858549,
          12149.76840536935,
          13051.381185913086,
          14724.000497671273,
          14506.134258563701,
          16132.371877034506,
          8778.623209272113
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
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144,
          26144
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
  "ParsedJson": {
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
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
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
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          41626.570294189456,
          41643.569763183594,
          40511.48933628627,
          1803.8105926513672,
          1007.3367557525635,
          1453.7463124593098,
          1594.2021198272705,
          1746.5741109114426,
          1910.2248825073243,
          1887.9338513783046,
          1825.8039059956868,
          1096.347824414571
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
          14342,
          14341,
          14342,
          2328,
          2328,
          2328,
          2328,
          2328,
          2328,
          2328,
          2328,
          1280
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
          14571.526543753487,
          14554.999634806316,
          13392.435609944661,
          12604.763811747232,
          6120.470632825579,
          9715.201798072228,
          11165.273520914714,
          12008.93495686849,
          13795.800675964356,
          13295.847416178385,
          14596.103698730469,
          8251.288558959961
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
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312,
          25312
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
  "JsonArrayElements": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          5381.484197889055,
          2517.031348301814
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
          6104,
          3584
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
          47792.53284505208,
          23825.67740275065
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
          61926,
          61921
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
  "JsonArrayInAnyOrder": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          40482.60314127604,
          23649.680282592773
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
          40368,
          29944
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
          296766.2811748798,
          175284.47587890626
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
          521594,
          521542
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
  "JsonEquality": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          3517.216092036321,
          1869.6961374282837
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
          3288,
          1600
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
          1881.7004268352803,
          1298.1013501485188
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
          2624,
          2624
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
  "JsonProperty": {
    "commits": [
      {
        "sha": "40ae946f89b5291fb97eca2e8896847f0a133467",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:23:57 2026 \u002B0200",
        "message": "test: cover the main features of the extension packages in their benchmarks (#1899)"
      },
      {
        "sha": "c2e176c2637dbde72d026ce0843cf4d1080d66ab",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 18:36:26 2026 \u002B0200",
        "message": "refactor: make JSON, HTTP and reflection expectations cheaper on the happy path (#1902)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          3573.886152903239,
          1689.8776786804199
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
          5528,
          3984
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
          1825.0977980749947,
          1361.7151435852052
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
          6224,
          6224
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