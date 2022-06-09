import React, { useState } from "react";
import Highcharts from "highcharts/highmaps";
import worldMap from "@highcharts/map-collection/custom/world.topo.json";
import HighchartsReact from "highcharts-react-official";

export const HighchartPage = (props) => {
  const options = {
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
  };

  console.dir(options);
  console.dir(Highcharts);
  return (React.createElement(HighchartsReact, { highcharts: Highcharts, constructorType: "mapChart", options: options }));
};