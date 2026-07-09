import React, { useMemo, memo } from "react";
import Highcharts from "highcharts/highmaps";
import worldMap from "@highcharts/map-collection/custom/world.topo.json";
import HighchartsReact from "highcharts-react-official";

const HighchartPageInner = (props) => {
  // Rebuild the Highcharts options (and thus reflow the map) only when the data
  // changes, not on every parent re-render.
  const options = useMemo(() => ({
    title: {
      text: ''
    },
    chart: {
      map: worldMap
    },
    mapNavigation: {
      enabled: true,
      buttonOptions: {
        alignTo: "spacingBox"
      }
    },
    mapView: {
      projection: {
        name: "WebMercator"
      }
    },
    colorAxis: {
      min: 0,
      stops: [
        [0, "#FaFEFE"],
        [0.05, "#66e3d0"],
        [0.25, "#62b9f3"],
        [1, "#9467bd"]
      ]
    },
    legend: {
      layout: "vertical",
      align: "left",
      verticalAlign: "bottom"
    },
    series: [
      {
        type: 'map',
        data: props.data,
        keys: ['value', 'hc-key'],
        joinBy: ["hc-key", "key"],
        name: "Tea bags",
        states: {
          hover: {
            color: "#f15c80"
          }
        },
        dataLabels: {
          enabled: false,
          // formatter,
          style: {
            fontWeight: 100,
            fontSize: "10px",
            textOutline: "none"
          }
        }
      },
    ]
  }), [props.data]);

  return (React.createElement(HighchartsReact, { highcharts: Highcharts, constructorType: "mapChart", options: options }));
};

// memo the inner component so it skips re-rendering when `data` is referentially
// unchanged. The F# interop calls HighchartPage(props) directly (not via
// createElement), so HighchartPage must stay a plain callable function that
// mounts the memoized inner as a real React element for the memo/useMemo to take.
const MemoHighchartPage = memo(HighchartPageInner);
export const HighchartPage = (props) => React.createElement(MemoHighchartPage, props);