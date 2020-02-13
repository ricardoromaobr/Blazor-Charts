﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblyMan.SVGRender;

namespace WebAssemblyMan
{
    //TODO
    /*
    1. Total as Label Number
    2. Input for Label Text
    */
    public class PieChart : ComponentBase
    {
        [Parameter]
        public string InputData { get; set; }

        [Parameter]
        public string InputLabels { get; set; }

        private double pieRadius = 0.85;    
        private void getCoordinatesForPercent(double percent, out double x, out double y)
        {
            x = pieRadius * Math.Cos(2 * Math.PI * percent);
            y = pieRadius * Math.Sin(2 * Math.PI * percent);

            Console.WriteLine("xx:"+ x);
            Console.WriteLine("yy:"+ y);

        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            string[] colors = { "#ce4b99", "#27A844", "#377bbc","#fe2712", "#fc600a", "#fb9902","#fccc1a", "#fefe33", "#b2d732", "#66b032", "#347c98", "#0247fe", "#4424d6","#8601af","#c21460" };
            string[] inputDataArr = InputData.Split(',');
            string[] inputLabelsArr = InputLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var seq = 0;
            builder.OpenElement(seq, "figure");
            builder.AddAttribute(++seq, "class", "pie-chart");
            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "main");

            //SVG svg = new SVG() { { "width", "100%" }, { "height", "100%" }, { "viewBox", "0 0 42 42" } };
            SVG svg = new SVG() { { "width", "100%" }, { "height", "100%" }, { "viewBox", "-1 -1 2 2" },{"style","transform: rotate(-90deg)" } };

            Rectangle rect = new Rectangle() { { "class", "background-rect" }, { "width", "100%" }, { "height", "100%" }, { "fill", "white" } };

            double x, y;
            double px=0, py=0;
            double totalPercent = 0;
            string prStr = pieRadius.ToString();

            for (int icounter=0; icounter < inputDataArr.Length; icounter++)
            {
                double percent = double.Parse(inputDataArr[icounter])/100;
                totalPercent = totalPercent + percent;
                getCoordinatesForPercent(totalPercent, out x, out y);
                Path path = null;
                if (icounter == 0)
                    path = new Path() { {"class","segment-"+(icounter+1).ToString()},  { "d", "M " + prStr + " 0 A " + prStr + " " + prStr + " 0 0 1 " + x + " " + y + " L 0 0" } };
                else 
                {
                    if (percent > 0.5)
                        path = new Path() { {"class","segment-"+(icounter+1).ToString()}, { "d", "M " + px + " " + py + " A " + prStr + " " + prStr + " 0 1 1 " + x + " " + y + " L 0 0" } };
                    else
                        path = new Path() { {"class","segment-"+(icounter+1).ToString()}, { "d", "M " + px + " " + py + " A " + prStr + " " + prStr + " 0 0 1 " + x + " " + y + " L 0 0" } };
                }

                svg.AddItems(path);
                px = x; py = y;

            }

            BlazorRenderer blazorRenderer = new BlazorRenderer();
            blazorRenderer.Draw(seq, builder, svg);
            
            builder.OpenElement(++seq, "figcaption");
            builder.AddAttribute(++seq, "class", "pie-key");
            builder.OpenElement(++seq, "ul");
            builder.AddAttribute(++seq, "class", "pie-key-list");
            builder.AddAttribute(++seq, "aria-hidden", "true");
            builder.AddAttribute(++seq, "style", "list-style-type: none;");

            int counter = 0;
            foreach (string dataStr in inputDataArr)
            {
                double data = double.Parse(dataStr);
                builder.OpenElement(++seq, "li");
                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "legend-dot-"+(counter+1).ToString());

                builder.CloseElement();

                string labels="";
                if (counter<inputLabelsArr.Length)
                {
                    labels=inputLabelsArr[counter];
                    counter++;
                }

                builder.AddContent(++seq, labels+" "+"("+data.ToString()+")");
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();
            

            builder.CloseElement();
            builder.CloseElement();

   

        }
    }
}
