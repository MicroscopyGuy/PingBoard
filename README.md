🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧
 <div align="center"> <b>**PingBoard: Under Construction**</b> </div>
🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧🚧

PingBoard is a dashboard application that sends out groups of pings to a specified IPAddress or domain. It is
written in C# using the ASP.NET Core framework and has a React-based user interface that I am currently 
working on.

The plan is to incorporate a charting library and package this all up as a desktop application using Electron.

One thing that is different about this pinging functionality is that it sends a configurable number of pings
every specified amount of time. This results in an inherent summary defined across the inverval of time
specified by the user.

What this means is that these PingGroupSummary objects have statistics that more expressively summarize
what happened in that interval of time, which will simultaneously reduce chart clutter and enable
a more granular analysis of the data.

The configurability of many aspects of pinging is already supported by the C# backend, including:
    A) Behavior: Responsiveness to TimeOuts, how many pings to send per group, etc
    B) Thresholds: When does the user consider the MaximumPing, Jitter, etc, over an interval to be too high?

If a user wishes to change the source code, they can even change the string sent in each packet. 
Currently every packet contains the open source license this project uses, as well as a link to this repo.

I'm excited to share more as I make more progress!

