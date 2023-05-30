/*
 Highstock JS v10.1.0 (2022-04-29)

 Indicator series type for Highcharts Stock

 (c) 2010-2021 Daniel Studencki

 License: www.highcharts.com/license
*/
(function(b){"object"===typeof module&&module.exports?(b["default"]=b,module.exports=b):"function"===typeof define&&define.amd?define("highcharts/indicators/acceleration-bands",["highcharts","highcharts/modules/stock"],function(l){b(l);b.Highcharts=l;return b}):b("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(b){function l(b,g,e,l){b.hasOwnProperty(g)||(b[g]=l.apply(null,e),"function"===typeof CustomEvent&&window.dispatchEvent(new CustomEvent("HighchartsModuleLoaded",{detail:{path:g,
module:b[g]}})))}b=b?b._modules:{};l(b,"Stock/Indicators/MultipleLinesComposition.js",[b["Core/Series/SeriesRegistry.js"],b["Core/Utilities.js"]],function(b,g){var e=b.seriesTypes.sma,l=g.defined,p=g.error,r=g.merge,k;(function(g){function n(a){var c,b=[];a=a||this.points;if(this.fillGraph&&this.nextPoints){if((c=e.prototype.getGraphPath.call(this,this.nextPoints))&&c.length){c[0][0]="L";b=e.prototype.getGraphPath.call(this,a);c=c.slice(0,b.length);for(var f=c.length-1;0<=f;f--)b.push(c[f])}}else b=
e.prototype.getGraphPath.apply(this,arguments);return b}function q(){var a=this,c=a.linesApiNames,d=a.areaLinesNames,f=a.points,h=a.options,w=a.graph,g={options:{gapSize:h.gapSize}},m=[],n=a.getTranslatedLinesNames(a.pointValKey),q=f.length,k;n.forEach(function(a,c){for(m[c]=[];q--;)k=f[q],m[c].push({x:k.x,plotX:k.plotX,plotY:k[a],isNull:!l(k[a])});q=f.length});if(this.userOptions.fillColor&&d.length){var x=n.indexOf(v(d[0]));x=m[x];d=1===d.length?f:m[n.indexOf(v(d[1]))];n=a.color;a.points=d;a.nextPoints=
x;a.color=this.userOptions.fillColor;a.options=r(f,g);a.graph=a.area;a.fillGraph=!0;b.seriesTypes.sma.prototype.drawGraph.call(a);a.area=a.graph;delete a.nextPoints;delete a.fillGraph;a.color=n}c.forEach(function(c,f){m[f]?(a.points=m[f],h[c]?a.options=r(h[c].styles,g):p('Error: "There is no '+c+' in DOCS options declared. Check if linesApiNames are consistent with your DOCS line names." at mixin/multiple-line.js:34'),a.graph=a["graph"+c],e.prototype.drawGraph.call(a),a["graph"+c]=a.graph):p('Error: "'+
c+" doesn't have equivalent in pointArrayMap. To many elements in linesApiNames relative to pointArrayMap.\"")});a.points=f;a.options=h;a.graph=w;e.prototype.drawGraph.call(a)}function d(a){var c=[];(this.pointArrayMap||[]).forEach(function(b){b!==a&&c.push(v(b))});return c}function v(a){return"plot"+a.charAt(0).toUpperCase()+a.slice(1)}function w(a){var c=[];(this.pointArrayMap||[]).forEach(function(b){c.push(a[b])});return c}function x(){var a=this,c=a.pointArrayMap,b=[],f;b=a.getTranslatedLinesNames();
e.prototype.translate.apply(a,arguments);a.points.forEach(function(d){c.forEach(function(c,w){f=d[c];a.dataModify&&(f=a.dataModify.modifyValue(f));null!==f&&(d[b[w]]=a.yAxis.toPixels(f,!0))})})}var k=[],z=["bottomLine"],A=["top","bottom"],B=["top"];g.compose=function(a){if(-1===k.indexOf(a)){k.push(a);var c=a.prototype;c.linesApiNames=c.linesApiNames||z.slice();c.pointArrayMap=c.pointArrayMap||A.slice();c.pointValKey=c.pointValKey||"top";c.areaLinesNames=c.areaLinesNames||B.slice();c.drawGraph=q;
c.getGraphPath=n;c.toYData=w;c.translate=x;c.getTranslatedLinesNames=d}return a}})(k||(k={}));return k});l(b,"Stock/Indicators/ABands/ABandsIndicator.js",[b["Stock/Indicators/MultipleLinesComposition.js"],b["Core/Series/SeriesRegistry.js"],b["Core/Utilities.js"]],function(b,g,e){var l=this&&this.__extends||function(){var b=function(e,d){b=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(b,d){b.__proto__=d}||function(b,d){for(var e in d)d.hasOwnProperty(e)&&(b[e]=d[e])};return b(e,d)};
return function(e,d){function g(){this.constructor=e}b(e,d);e.prototype=null===d?Object.create(d):(g.prototype=d.prototype,new g)}}(),p=g.seriesTypes.sma,r=e.correctFloat,k=e.extend,y=e.merge;e=function(b){function e(){var d=null!==b&&b.apply(this,arguments)||this;d.data=void 0;d.options=void 0;d.points=void 0;return d}l(e,b);e.prototype.getValues=function(d,e){var g=e.period,k=e.factor;e=e.index;var l=d.xData,n=(d=d.yData)?d.length:0,p=[],q=[],a=[],c=[],v=[],f;if(!(n<g)){for(f=0;f<=n;f++){if(f<n){var h=
d[f][2];var t=d[f][1];var u=k;h=r(t-h)/(r(t+h)/2)*1E3*u;p.push(d[f][1]*r(1+2*h));q.push(d[f][2]*r(1-2*h))}if(f>=g){h=l.slice(f-g,f);var m=d.slice(f-g,f);u=b.prototype.getValues.call(this,{xData:h,yData:p.slice(f-g,f)},{period:g});t=b.prototype.getValues.call(this,{xData:h,yData:q.slice(f-g,f)},{period:g});m=b.prototype.getValues.call(this,{xData:h,yData:m},{period:g,index:e});h=m.xData[0];u=u.yData[0];t=t.yData[0];m=m.yData[0];a.push([h,u,m,t]);c.push(h);v.push([u,m,t])}}return{values:a,xData:c,yData:v}}};
e.defaultOptions=y(p.defaultOptions,{params:{period:20,factor:.001,index:3},lineWidth:1,topLine:{styles:{lineWidth:1}},bottomLine:{styles:{lineWidth:1}},dataGrouping:{approximation:"averages"}});return e}(p);k(e.prototype,{areaLinesNames:["top","bottom"],linesApiNames:["topLine","bottomLine"],nameBase:"Acceleration Bands",nameComponents:["period","factor"],pointArrayMap:["top","middle","bottom"],pointValKey:"middle"});b.compose(e);g.registerSeriesType("abands",e);"";return e});l(b,"masters/indicators/acceleration-bands.src.js",
[],function(){})});
//# sourceMappingURL=acceleration-bands.js.map