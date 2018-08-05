# FileShare-Tracker
Pair of apps to track files across multiple sharers and facilitate their parallelised download.

Workflow:

1) Tracker starts listening.

2) FileSharerDownloaders choose folders to share, set a port to listen to initiate transfers and send share-list to the tracker

3) A FileSharerDownloader sets a download folder and makes a filename request from tracker.

4) Tracker repilies with a list of all FileSharerDownloaders that have the requested file.

5) The FileSharerDownloader requests a small part of the requested file from each of the peers that have the file.

6) Download completes.
