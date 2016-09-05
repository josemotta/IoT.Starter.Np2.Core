<h1>IoT Starter for Netduino Plus 2 - Core</h1>

<h2>Introduction</h2>

<p>In the world there are tenths billions of smart devices connected to the Internet right now, including all sizes of computers, tablets, cell phones, smart watches, pagers, etc. The IoT technology expects to double this quantity before 2020, which represents a $6 trillion market to be developed in the next few years.</p>

<p>IoT development challenges us to behave differently, compared to current computing environment, based on human interaction with screen, touch, keyboard and mouse. IoT devices should interact directly with nature, processing data in a real time basis.</p>

<p>The development process is expected to be more complex and may require proper tools for developers. Did you consider buying an oscilloscope for debugging? In order to speed up the IoT development, this article introduces a IoT Starter Kit that may help to speed up an IoT initiative.</p>

<p>Sources&nbsp;are found at Github repository <strong itemprop="name" style="font-weight: 600; box-sizing: border-box;"><a data-pjax="#js-repo-pjax-container" href="https://github.com/josemotta/IoT.Starter.Np2.Core" style="color: rgb(64, 120, 192); text-decoration: none; box-sizing: border-box; background-color: transparent;">IoT.Starter.Np2.Core</a>.</strong></p>

<h2>Background</h2>

<p>Supposing you are trying to become an IoT entrepreneur, how to minimize the risks for an IoT initiative? How to satisfy users, investors and decision makers that support a brilliant &quot;idea&quot; for an IoT project? Some steps may help to reduce the risks:</p>

<ul>
	<li>Business Plan</li>
	<li>Team</li>
	<li>Methodology</li>
	<li>Proof of Concept</li>
	<li>Starter Kit</li>
	<li>Pilot</li>
	<li>Commercial</li>
</ul>

<p>Then, suppose the original &quot;idea&quot; has grown enough to fit in a profitable business plan and it was accepted by investors. After that, an executive manager get involved and gathered a compact developer team, well trained to apply an agile methodology that assures&nbsp;a smooth and continuous workflow. Great! Where are we now?</p>

<p>Some time has passed since the investors came in and they are already pushing for results. We should speed up the development process to launch product as soon as possible, they say.</p>

<p>We just started Proof of Concept, when we expect to build the preliminary version that comply with users and investors needs. This is the first version and it should not handle all the complexity required by the final product. The goal is to generate a healthy embryo. Since methodology may evolve the embryo continuously, we can start it small and each version will add functionality and new tests until we have the Pilot prototype.</p>

<p>This is the moment that a Starter Kit may help the team to focus in the main concepts behind the IoT initiative and make it profitable. Better than that, the resources we got until now - a manager leading a team applying a methodology to generate a smooth workflow, all them are fully operational, ready to run,&nbsp;immediately after the Starter Kit is installed.</p>

<h2>Building the Starter Kit</h2>

<p>Since IoT requires smart devices connected to the Internet, we should choose the product platform, which means a <code>CPU </code>and a <code>network interface</code>. There are many nice ARM platforms today, including Arduino, Netduino and Raspberry PI, among others. The latest models added wired and/or wireless network interfaces to the processor boards, keeping the low cost. Naturally, we should get the best one that&nbsp;fit&nbsp;our budget.</p>

<p>We should also consider that hardware and software for the product platform should be kept minimal, just to fit the IoT initiative. Most cases include a custom hardware, maybe a daughter board or a shield added to the Arduino bus. Simulating the team workflow, we conclude that most of their time will be spent programming and testing a software application being executed on the target platform, interacting with the customized hardware.</p>

<p>Then,&nbsp;we need an external development system, installed on a microcomputer, in order to have:</p>

<ul>
	<li>a programming <code>language </code>and its compiler generating code for&nbsp;the product;</li>
	<li><code>utilities </code>to load the executable code and debug it on the target platform;</li>
	<li>a library containing <code>communication </code>software, providing basic Internet&nbsp;services;</li>
	<li>a library with other <code>utilities</code>, for disk storage, for example;</li>
	<li>software library&nbsp;<code>modules</code>, developed for&nbsp;IoT, like IR codecs or sensor/motor drivers.</li>
</ul>

<p>This first cenario chooses the Netduino Plus 2, an open-source electronics platform using the .NET Micro Framework that runs at 168 MHz with 1 MB Flash and 192 KB RAM.&nbsp;With&nbsp;Netduino,&nbsp;we can benefit from the MS Visual Studio as the external development system. We&nbsp;may also&nbsp;have the C#&nbsp;programming language, its&nbsp;great compiler&nbsp;and the corresponding loader and debugger. The&nbsp;.NET software libraries also provide the utilities we need to handle disks and other periferals we may have. Another <a href="http://www.codeproject.com/Articles/840511/Introduction-to-Netduino-Plus-with-examples">article </a>from Guruprasad&nbsp;detailed the Netduino Plus 2.</p>

<p>As <a href="http://www.netduino.com/downloads/">recommended</a>, install the development environment&nbsp;in the following order:&nbsp;</p>

<p>&nbsp;&nbsp; 1)&nbsp; Microsoft Visual Studio Express 2013<br />
&nbsp;&nbsp; 2) .NET Micro Framework SDK v4.3<br />
&nbsp;&nbsp; 3) .NET MF plug-in for VS2013<br />
&nbsp;&nbsp; 4) Netduino SDK v4.3.2.1</p>

<p>Download the repository at&nbsp;<strong itemprop="name" style="font-weight: 600; box-sizing: border-box;"><a data-pjax="#js-repo-pjax-container" href="https://github.com/josemotta/IoT.Starter.Np2.Core" style="color: rgb(64, 120, 192); text-decoration: none; box-sizing: border-box; background-color: transparent;">IoT.Starter.Np2.Core</a></strong>, there are four VS projects at <code>Starter </code>folder:</p>

<ul>
	<li>Controller: contains the main application,&nbsp;please check&nbsp;Program.cs;</li>
	<li>FileServer: is a Http file server that&nbsp;listen to clients and process List, Get Put and Delete operations on files stored at&nbsp;the micro SD disk;</li>
	<li>HttpLibraryV3: include a basic Http Server with corresponding HttpRequest and HttpResponse calls. Also add a NtpClient utility to&nbsp;initialize&nbsp;board&acute;s date and time directly from the Internet.</li>
</ul>

<h2>Using the code</h2>

<p>A brief description of how to use the article or code. The class names, the methods and properties, any tricks or tips.Blocks of code should be set as style &quot;Formatted&quot; like this:</p>

<pre lang="cs">
//
// Any source code blocks look like this
//</pre>

<p>&nbsp;</p>

<p>Remember to set the Language of your code snippet using the Language dropdown.</p>

<p>Use the &quot;var&quot; button to to wrap Variable or class names in &lt;code&gt; tags like <code>this</code>.</p>

<h2>Points of Interest</h2>

<p>Did you learn anything interesting/fun/annoying while writing the code? Did you do anything particularly clever or wild or zany?</p>

<h2>History</h2>

<p>Keep a running update of any changes or improvements you&#39;ve made here.</p>