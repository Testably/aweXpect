window.BENCHMARK_DATA = {
  "Content": {
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
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
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          240.23763929094588,
          248.64943994794572,
          491.37959524790443,
          386.78248827798024,
          520.1031948498318,
          511.06023858388266,
          211.7148337841034,
          207.94492700894673,
          403.7941200182988,
          340.85249172846477,
          400.3086359024048,
          310.4744209766388,
          194.59093516213554,
          397.7752680097307,
          413.761456934611
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
          944,
          944,
          944,
          944,
          944,
          944,
          728,
          728,
          728,
          728,
          728,
          728,
          728,
          664,
          664
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
          479.16028465543474,
          506.0401243796715,
          962.7225375856672,
          803.6286273956299,
          1046.7631839752198,
          1010.8425723484585,
          543.9756457989032,
          569.9150924682617,
          959.2850583394369,
          831.7780774661472,
          950.9255721909659,
          768.0385799407959,
          488.63950770696005,
          950.0418487548828,
          949.0095469156901
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
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
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          132.2429928439004,
          141.93751222746712,
          255.7064263343811,
          209.88934963941574,
          311.15056200027465,
          271.12410670060376,
          115.1127900821822,
          105.97076732771737,
          193.97291277249653,
          166.5883249123891,
          199.08713877995808,
          172.49066912210904,
          106.72830493633563,
          188.33495831489563,
          185.9222469329834
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
          784,
          784,
          784,
          784,
          784,
          784,
          568,
          568,
          568,
          568,
          568,
          568,
          568,
          504,
          504
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
          132.73189579645793,
          140.00120902061462,
          245.2166986465454,
          212.2653868538993,
          285.89191370010377,
          265.13456317356656,
          142.2578024693898,
          131.59890479700906,
          248.3692032269069,
          220.05329035123188,
          256.2028199513753,
          227.57875340779623,
          132.7498309135437,
          239.14522199630738,
          248.61020447413128
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          698.5639862060547,
          508.6798000971476,
          354.9212050437927,
          628.3123481114706,
          694.9000593821207
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
          1208,
          1208,
          1208,
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
          1246.2395397186278,
          742.6188469568889,
          459.5191803659712,
          927.7227094014486,
          936.2941907735972
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1683.6093157359533,
          1068.9163294474283,
          990.9442194620768,
          1661.2446229117256,
          1658.5810256958007
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
          1952,
          1792,
          1792,
          1728,
          1728
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
          20474.48770359584,
          12062.250297546387,
          9783.402879842122,
          20622.016970120945,
          20800.499821589543
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1382.2916792551675,
          634.8672311782836,
          446.43845116297405,
          887.9110375813076,
          858.903605052403
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
          2944,
          1456,
          1456,
          1408,
          1408
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
          1290.2306609834943,
          990.8289800371442,
          678.8305410385132,
          1266.8902228037516,
          1316.8672934214273
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
      },
      {
        "sha": "f8bfa2a95dcd676c83965667eb45d8760cd3714f",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:04 2026 \u002B0200",
        "message": "fix: honor comparison predicates for primitive members of registered types (#1898)"
      },
      {
        "sha": "c0e6021947a1ec7c816474575ea4451a4e36c0b7",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 21:21:42 2026 \u002B0200",
        "message": "refactor: allocate less for every expectation, equivalency and combined expectations (#1900)"
      },
      {
        "sha": "706259ecd2142378bb9c8136fb0712d8354d41fa",
        "author": "Valentin Breu\u00DF",
        "date": "Tue Oct 20 22:25:16 2026 \u002B0200",
        "message": "docs: show the benchmark results of the extension packages (#1905)"
      }
    ],
    "labels": [
      "40ae946f",
      "c2e176c2",
      "f8bfa2a9",
      "c0e60219",
      "706259ec"
    ],
    "datasets": [
      {
        "label": "aweXpect time",
        "unit": "ns",
        "data": [
          1386.2848340352377,
          894.9806112925212,
          861.3152570724487,
          1351.3349540710449,
          1378.839906946818
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
          1832,
          1832,
          1832,
          1664,
          1664
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
          758.2722127278646,
          617.3273561341422,
          426.7081581865038,
          729.8756811278207,
          734.6849997202555
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