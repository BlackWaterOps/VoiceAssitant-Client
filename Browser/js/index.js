// Generated by CoffeeScript 1.6.3
(function(){var e=function(e,t){return function(){return e.apply(t,arguments)}};window.Please=function(){function n(t){this.error=e(this.error,this);this.log=e(this.log,this);this.formatDate=e(this.formatDate,this);this.elapsedTimeHelper=e(this.elapsedTimeHelper,this);this.datetimeHelper=e(this.datetimeHelper,this);this.newDateHelper=e(this.newDateHelper,this);this.fuzzyHelper=e(this.fuzzyHelper,this);this.weekdayHelper=e(this.weekdayHelper,this);this.buildDatetime=e(this.buildDatetime,this);this.replaceDates=e(this.replaceDates,this);this.replaceLocation=e(this.replaceLocation,this);this.prependTo=e(this.prependTo,this);this.clientOperations=e(this.clientOperations,this);this.toISOString=e(this.toISOString,this);this.requestHelper=e(this.requestHelper,this);this.buildDeviceInfo=e(this.buildDeviceInfo,this);this.nameMap=e(this.nameMap,this);this.replace=e(this.replace,this);this.find=e(this.find,this);this.reduce=e(this.reduce,this);this.updatePosition=e(this.updatePosition,this);this.getLocation=e(this.getLocation,this);this.show=e(this.show,this);this.addDebug=e(this.addDebug,this);this.actorResponseHandler=e(this.actorResponseHandler,this);this.actor=e(this.actor,this);this.auditorSuccessHandler=e(this.auditorSuccessHandler,this);this.auditor=e(this.auditor,this);this.handleChoice=e(this.handleChoice,this);this.choose=e(this.choose,this);this.disambiguatePassive=e(this.disambiguatePassive,this);this.disambiguatePersonal=e(this.disambiguatePersonal,this);this.disambiguateActive=e(this.disambiguateActive,this);this.disambiguateSuccessHandler=e(this.disambiguateSuccessHandler,this);this.classify=e(this.classify,this);this.replaceContext=e(this.replaceContext,this);this.cancel=e(this.cancel,this);this.expand=e(this.expand,this);this.keyup=e(this.keyup,this);this.ask=e(this.ask,this);this.registerHandlebarHelpers=e(this.registerHandlebarHelpers,this);this.registerListeners=e(this.registerListeners,this);this.init=e(this.init,this);this.debug=!0;this.debugData={};this.classifier="http://casper-cached.stremor-nli.appspot.com/v1";this.disambiguator="http://casper-cached.stremor-nli.appspot.com/v1/disambiguate";this.personal="http://stremor-pud.appspot.com/v1/";this.responder="http://rez.stremor-apier.appspot.com/v1/";this.lat=33.4930947;this.lon=-111.928558;this.mainContext=null;this.disambigContext=null;this.history=[];this.pos=this.history.length;this.loader=$(".loader");this.board=$(".board");this.input=$(".main-input");this.dateRegex=/\d{2,4}[-]\d{2}[-]\d{2}/i;this.timeRegex=/\d{1,2}[:]\d{2}[:]\d{2}/i;this.counter=0;this.disableSpeech=!1;this.currentState={status:null,origin:null};this.presets={"after work":"18:00:00",breakfast:"7:30:00",lunch:"12:00:00"};this.init()}var t;n.prototype.init=function(){var e;this.getLocation();this.registerHandlebarHelpers();this.registerListeners();this.input.focus().on("webkitspeechchange",this.ask).on("keyup",this.keyup);$("body").on("click",".expand",this.expand).on("click",".choice-item",this.handleChoice);$("#cancel").on("click",this.cancel);if(this.board.is(":empty")){e=$(".init");e.fadeIn("slow");return setTimeout(function(){return e.fadeOut("slow")},1e3)}};n.prototype.registerListeners=function(){return $(document).on("init",this.classify).on("audit",this.auditor).on("disambiguate",this.disambiguatePassive).on("disambiguate:personal",this.disambiguatePersonal).on("disambiguate:active",this.disambiguateActive).on("restart",this.replaceContext).on("choice",this.choose).on("inprogress",this.show).on("completed",this.actor).on("error",this.show).on("debug",this.addDebug)};n.prototype.registerHandlebarHelpers=function(){var e=this;Handlebars.registerHelper("elapsedTime",function(t){var n;n=e.elapsedTimeHelper(t);return n.newDate+" "+n.newTime});Handlebars.registerHelper("flightDates",function(t,n){var r,i,s,o,u,a,f,l,c,h;if(t==null)return"--";o=e.formatDate(t);r=o.am;f=o.month;s=o.date;h=o.year;u=o.hours;a=o.minutes;i=o.dayOfWeek;l=o.monthOfYear;c='<span class="formatted-time">'+u+":"+a+'</span><span class="formatted-date">'+i+", "+l+" "+s+", "+h+"</span>";return new Handlebars.SafeString(c)});return Handlebars.registerHelper("eventDates",function(t){var n,r,i,s,o,u;i=e.formatDate(t);s=i.month;r=i.date;u=i.year;n=i.dayOfWeek;o=i.monthOfYear;return n.substr(0,3)+", "+o.substr(0,3)+" "+r+", "+u})};n.prototype.ask=function(e){var t,n;if(typeof e=="string")n=e;else{e=$(e);n=e.val();e.val("")}t=Handlebars.compile($("#bubblein-template").html());this.board.append(t(n)).scrollTop(this.board.find(".bubble:last").offset().top);$(".input-form").addClass("cancel");return this.currentState.state==="inprogress"||this.currentState.state==="error"&&this.disambigContext!=null?this.currentState.origin==="actor"?$(document).trigger({type:"completed",response:null}):$(document).trigger({type:"disambiguate:active",response:n}):$(document).trigger({type:"init",response:n})};n.prototype.keyup=function(e){var t,n;n=$(e.target).val();t=$(e.target);switch(e.which){case 13:if(n){this.ask(t);this.history.push(n);return this.pos=this.history.length}break;case 38:this.pos>0&&(this.pos-=1);return t.val(this.history[this.pos]);case 40:this.pos<this.history.length&&(this.pos+=1);return t.val(this.history[this.pos])}};n.prototype.expand=function(e){e.preventDefault();return $(e.target).parent().next().toggle()};n.prototype.cancel=function(e){this.board.empty();this.mainContext=null;this.disambigContext=null;this.history=[];this.currentState={state:null,origin:null};$(".input-form").removeClass("cancel");this.loader.hide();this.counter=0;return this.input.focus()};n.prototype.store={createCookie:function(e,t,n){var r,i,s;r=new Date;r.setDate(r.getDate()+n);i=escape(t)+((s=exdays===null)!=null?s:{"":"; expires="+r.toUTCString()});document.cookie=e+"="+t;return!0},get:function(e){var t,n,r;if(window.Modernizr.localstorage)return $.parseJSON(localStorage.getItem(e));t=0;while(t<c.length){n=c[t].substr(0,c[t].indexOf("="));r=c[t].substr(c[t].indexOf("=")+1);n=n.replace(/^\s+|\s+$/g,"");if(n===e)return unescape(r);c++}},set:function(e,t){return window.Modernizr.localstorage?localStorage.setItem(e,JSON.stringify(t)):createCookie(e,t,365)},remove:function(e){return window.Modernizr.localstorage?localStorage.removeItem(e):createCookie(e,v,-1)},clear:function(){return window.Modernizr.localstorage?localStorage.clear():createCookie(k,v,-1)}};n.prototype.replaceContext=function(e){var t;this.counter=0;t=e.response;this.mainContext=t.data;return this.auditor(this.mainContext)};n.prototype.classify=function(e){var t,n,r=this;n=e instanceof $.Event?e.response:e;t={query:n};return this.requestHelper(this.classifier,"GET",t,function(e){return $(document).trigger($.Event("debug")).trigger({type:"audit",response:e})})};n.prototype.disambiguateSuccessHandler=function(e,t,n){var r;(this.currentState.state==="inprogress"||this.currentState.state==="error")&&$(document).trigger($.Event("debug"));if(e!=null){this.clientOperations(e);t.indexOf(".")!==-1?this.replace(t,e[n]):this.mainContext.payload[t]=e[n];r=$.extend({},this.mainContext);return this.auditor(r)}return console.log("oops no responder response",results)};n.prototype.disambiguateActive=function(e){var t,n,r,i,s=this;t=this.disambigContext.field;i=this.disambigContext.type;r=e.response;n={payload:r,type:i};return this.requestHelper(this.disambiguator+"/active","POST",n,function(e){return s.disambiguateSuccessHandler(e,t,i)})};n.prototype.disambiguatePersonal=function(e){var t,n,r,i,s,o=this;t=e.response;n=t.field;s=t.type;n.indexOf(".")!==-1?i=this.find(n):i=this.mainContext.payload[n];r={type:s,payload:i};return this.requestHelper(this.personal+"disambiguate","POST",r,function(e){return o.disambiguateSuccessHandler(e,n,s)})};n.prototype.disambiguatePassive=function(e){var t,n,r,i,s,o=this;t=e.response;n=t.field;s=t.type;n.indexOf(".")!==-1?i=this.find(n):i=this.mainContext.payload[n];r={type:s,payload:i};return this.requestHelper(this.disambiguator+"/passive","POST",r,function(e){return o.disambiguateSuccessHandler(e,n,s)})};n.prototype.choose=function(e){var t,n,r,i,s,o,u;t=e.response;r=$("<ul/>");u=t.show.simple.list;for(s=0,o=u.length;s<o;s++){n=u[s];i=$("<li/>").addClass("choice-item").data("choice",n).append($("<a/>").text(n.text));r.append(i)}$(".list-slider").html(r);$("body").addClass("choice");return this.show(e)};n.prototype.handleChoice=function(e){var t,n,r;$("body").removeClass("choice");t=$(e.currentTarget).data("choice");n=this.disambigContext.field;r=Handlebars.compile($("#bubblein-template").html());this.board.append(r(t.text)).scrollTop(this.board.find(".bubble:last").offset().top);n.indexOf(".")!==-1?this.replace(n,t):this.mainContext.payload[n]=t;return $(document).trigger({type:"audit",response:this.mainContext})};n.prototype.auditor=function(e){var t,n;n=e instanceof $.Event?e.response:e;t=n.payload;t!=null&&this.clientOperations(t);this.mainContext=n;this.counter++;if(this.counter<3)return this.requestHelper(this.responder+"audit","POST",n,this.auditorSuccessHandler)};n.prototype.auditorSuccessHandler=function(e){this.currentState={state:e.status.replace(" ",""),origin:"auditor"};if(this.currentState.state==="inprogress"||this.currentState.state==="choice")this.disambigContext=e;return $(document).trigger({type:this.currentState.state,response:e})};n.prototype.actor=function(e){var t,n;this.disambigContext={};t=e.response;if(t.actor===null||t.actor===void 0)return this.show(t);n=this.responder+"actors/"+t.actor;if(t.actor.indexOf("private")!==-1){t.actor=t.actor.replace("private:","");n=this.personal+"actors/"+t.actor}return this.requestHelper(n,"POST",this.mainContext,this.actorResponseHandler)};n.prototype.actorResponseHandler=function(e){if(e.status!=null){this.currentState={state:e.status.replace(" ",""),origin:"actor"};return $(document).trigger({type:this.currentState.state,response:e})}return this.show(e)};n.prototype.addDebug=function(e){var t,n;if(this.debug===!1)return;this.debugData.request!=null&&(this.debugData.request=JSON.stringify(this.debugData.request,null,4));this.debugData.response!=null&&(this.debugData.response=JSON.stringify(this.debugData.response,null,4));t=e.response;t?t.debug=this.debugData:t={debug:this.debugData};n=Handlebars.compile($("#debug-template").html());return this.board.find(".bubble:last").append(n(t))};n.prototype.show=function(e){var t,n,r,i,s;e instanceof $.Event&&(e=e.response);r=e.show.simple;r.link!=null?i="link":r.image!=null?i="image":i="bubbleout";t=$("#"+i+"-template");t=Handlebars.compile(t.html());this.board.append(t(r)).scrollTop(this.board.find(".bubble:last").offset().top);$(document).trigger({type:"debug",response:e});if(e.show!=null&&e.show.structured!=null&&e.show.structured.template!=null){r=e.show.structured.items;t=e.show.structured.template.split(":");n=t[0];s=t[1];i=t[2]!=null?t[2]:s;t=$("#"+s+"-template");t.length===0&&(t=$("#"+n+"-template"));if(t.length>0){t=Handlebars.compile(t.html());this.board.find(".bubble:last").append(t(r)).scrollTop(this.board.find(".bubble:last").offset().top)}}this.loader.hide();this.counter=0;return window.top.postMessage({action:"speak",speak:e.speak,options:{}},"*")};n.prototype.getLocation=function(){return navigator.geolocation.getCurrentPosition(this.updatePosition)};n.prototype.updatePosition=function(e){this.lat=e.coords.latitude;return this.lon=e.coords.longitude};n.prototype.reduce=function(e,t,n){if(t.length>0){n=e(n,t[0]);return this.reduce(e,t.slice(1),n)}return n};n.prototype.find=function(e){var t,n=this;t=e.split(".");return"function"==typeof Array.prototype.reduce?t.reduce(function(e,t,n,r){return e[t]||null},this.mainContext):this.reduce(function(e,t){return e[t]||null},t,this.mainContext)};n.prototype.replace=function(e,t){var n,r,i;n=e.split(".");r=n.pop();e=n.join(".");i=this.find(e);return i[r]=t};n.prototype.nameMap=function(e){var t;t=!1;e.indexOf(this.disambiguator)!==-1?t="Disambiguator":e.indexOf(this.classifier)!==-1?t="Casper":e.indexOf(this.responder)!==-1?t="Rez":e.indexOf(this.personal)!==-1&&(t="Pud");return t};n.prototype.buildDeviceInfo=function(){var e,t;e=new Date;return t={latitude:this.lat,longitude:this.lon,timestamp:e.getTime()/1e3,timeoffset:-e.getTimezoneOffset()/60}};n.prototype.requestHelper=function(e,t,n,r){var i,s=this;if(e.indexOf(this.disambiguator)!==-1&&e.indexOf("passive")!==-1){n=$.extend({},n);n.device_info=this.buildDeviceInfo()}this.debug===!0&&(this.debugData={endpoint:e,type:t,request:n});i=this.nameMap(e);return $.ajax({url:e,type:t,data:t==="POST"?JSON.stringify(n):n,dataType:"json",timeout:2e4,beforeSend:function(){s.log(i,">",n);return s.loader.show()}}).done(function(e,t){s.log(i,"<",e);if(s.debug===!0){s.debugData.status=t;s.debugData.response=e}if(r!=null)return r(e)}).fail(function(e,t){s.error(i,"<",e.responseJSON!=null?e.responseJSON:e);if(s.debug===!0){s.debugData.status=t;s.debugData.response=e}s.loader.hide();return s.counter=0})};t={"+":function(e,t){return parseInt(e,10)+parseInt(t,10)},"-":function(e,t){return parseInt(e,10)-parseInt(t,10)}};n.prototype.toISOString=function(e){var t;t=function(e){var t;t=String(e);t.length===1&&(t="0"+t);return t};return e.getFullYear()+"-"+t(e.getMonth()+1)+"-"+t(e.getDate())+"T"+t(e.getHours())+":"+t(e.getMinutes())+":"+t(e.getSeconds())};n.prototype.clientOperations=function(e){this.replaceLocation(e);this.replaceDates(e);if(e.unused_tokens!=null)return this.prependTo(e)};n.prototype.prependTo=function(e){var t,n,r;r=e.unused_tokens.join(" ");t=e.prepend_to;n=this.mainContext.payload[t];n=n==null?"":" "+n;return this.mainContext.payload[t]=r+n};n.prototype.replaceLocation=function(e){if(e!=null&&e.location!=null)switch(e.location){case"#current_location":e.location=this.buildDeviceInfo()}};n.prototype.replaceDates=function(e){var t,n,r,i,s,o,u;r=[["date","time"],["start_date","start_time"],["end_date","end_time"]];for(o=0,u=r.length;o<u;o++){i=r[o];t=i[0];s=i[1];if(e[t]!=null||e[s]!=null){n=this.buildDatetime(e[t],e[s]);if(n!=null){e[t]!=null&&(e[t]=n.date);e[s]!=null&&(e[s]=n.time)}}}};n.prototype.buildDatetime=function(e,t){var n,r;r=null;e!==null&&e!==void 0&&this.dateRegex.test(e)===!1&&(r=this.datetimeHelper(e));t!==null&&t!==void 0&&this.timeRegex.test(t)===!1&&(r=this.datetimeHelper(t,r));if(r==null)return;n=this.toISOString(r).split("T");return{date:n[0],time:n[1]}};n.prototype.weekdayHelper=function(e){var t,n,r,i;r=new Date;n=r.getDay();t=r.getDate();i=n<e?e-n:7-(n-e);r.setDate(t+i);return r};n.prototype.fuzzyHelper=function(e,t){var n,r,i,s,o,u,a,f,l;n=new Date;o=null;i=null;for(s in e){l=e[s];s==="label"&&(o=l);s==="default"&&(i=l)}a=this.presets[o];u=a!=null?a:i;if(u===null)return;f=t===!0?"-":":";r=u.trim().split(f);if(t===!0){n.setFullYear(r[0]);n.setMonth(r[1]-1);n.setDate(r[2])}else{n.setHours(r[0]);n.setMinutes(r[1]);n.setSeconds(r[2])}return n};n.prototype.newDateHelper=function(e){var t,n,r,i,s;if(e.indexOf("now")!==-1)r=new Date;else if(this.dateRegex.test(e)===!0){s=e.split("-");r=new Date(s[0],s[1]-1,s[2])}else if(this.timeRegex.test(e)===!0){r=new Date;s=e.split(":");t=r.getHours();n=r.getMinutes();i=r.getSeconds();(t>s[0]||t===s[0]&&n>s[1])&&r.setDate(r.getDate()+1);r.setHours(s[0]);r.setMinutes(s[1]);r.setSeconds(s[2])}return r===null||r===void 0?new Date:r};n.prototype.datetimeHelper=function(e,n){var r,i,s,o,u,a,f,l,c,h,p,d,v,m,g,y;n==null&&(n=null);o=Object.prototype.toString.call(e);switch(o){case"[object String]":n===null&&(n=this.newDateHelper(e));break;case"[object Object]":for(r in e){d=e[r];if(r.indexOf("weekday")!==-1)return this.weekdayHelper(d);if(r.indexOf("fuzzy")!==-1){a=r.indexOf("date")!==-1?!0:!1;return this.fuzzyHelper(d,a)}p=r.indexOf("add")!==-1?"+":"-";v=Object.prototype.toString.call(d);if(v==="[object Array]")for(g=0,y=d.length;g<y;g++){f=d[g];c=Object.prototype.toString.call(f);if(n===null)switch(c){case"[object String]":n=this.newDateHelper(f);break;case"[object Object]":for(l in f){h=f[l];if(n===null)if(l.indexOf("weekday")!==-1)n=this.weekdayHelper(h);else if(l.indexOf("fuzzy")!==-1){a=l.indexOf("date")!==-1?!0:!1;n=this.fuzzyHelper(h,a)}}}else if(c==="[object Number]"){u=f;if(u===null)return;if(r.indexOf("time")!==-1){i=n.getSeconds();m=t[p](i,u);n.setSeconds(m)}else if(r.indexOf("date")!==-1){i=n.getDate();s=t[p](i,u);n.setDate(s)}}}}}return n};n.prototype.elapsedTimeHelper=function(e){var t,n,r,i,s,o,u,a,f,l;r=this.formatDate(e);u=r.month+"/"+r.date+"/"+r.year;a=r.hours+":"+r.minutes;i=u;s=a;n=new Date(e);l=new Date;t=n.getTime();f=l.getTime();if(f-t<864e5){u="";o=Math.round((f-t)/1e3/60);if(o<60)a="About "+o+" minutes ago";else{o=Math.round(o/60);o===1?a="About "+o+" hour ago":a="About "+o+" hours ago"}}return{oldDate:s,oldTime:i,newDate:u,newTime:a}};n.prototype.formatDate=function(e){var t,n,r,i,s,o,u,a,f,l,c;l=["January","February","March","April","May","June","July","August","September","October","November","December"];i=["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];o=new Date(e);f=o.getMonth()+1;s=o.getDate();c=o.getFullYear();u=o.getHours();a=o.getMinutes();r=o.getDay();f<10&&(f="0"+f);s<10&&(s="0"+s);t=u>12?"pm":"am";u>12&&(u-=12);u<10&&(u="0"+u);a<10&&(a="0"+a);return n={year:c,month:f,monthOfYear:l[o.getMonth()],date:s,day:r,dayOfWeek:i[r],hours:u,minutes:a,am:t}};n.prototype.log=function(){var e,t,n,r;e=[];for(n=0,r=arguments.length;n<r;n++){t=arguments[n];typeof t=="object"&&(t=JSON.stringify(t,null," "));e.push(t)}return console.log(e.join(" "))};n.prototype.error=function(){var e,t,n,r;e=[];for(n=0,r=arguments.length;n<r;n++){t=arguments[n];typeof t=="object"&&(t=JSON.stringify(t,null," "));e.push(t)}return console.error(e.join(" "))};return n}()}).call(this);