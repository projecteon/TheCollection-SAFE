
$AzCopy='C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe'
$Source='/Source:C:\temp\Theedatabase\Afbeeldingen Zakjes'
$SourceThumb='/Source:C:\temp\teabagthumbs2'
# $MissingSource='/Source:C:\src\projecteon\missingthees'
# $DestinationEmu='/Dest:http://127.0.0.1:10000/devstoreaccount1/images'
# $DestinationEmuKey='Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=='
# & $AzCopy $Source $DestinationEmu /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationEmuKey /SetContentType:image/jpeg

$Destination='/Dest:https://collectionteststorage.blob.core.windows.net/images/'
$DestinationKey='XjMZ94aWTJ7LcpmhdC8c0Dc8WQahrJofVaLksxlKgsfJBhKh+KksjX1ULYO+sn81jtOSQQ6k9SJnf7wZfQfaxw=='
& $AzCopy $Source $Destination /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationKey /SetContentType:image/jpeg

$DestinationThumb='/Dest:https://collectionteststorage.blob.core.windows.net/thumbnails/'
$DestinationThumbKey='XjMZ94aWTJ7LcpmhdC8c0Dc8WQahrJofVaLksxlKgsfJBhKh+KksjX1ULYO+sn81jtOSQQ6k9SJnf7wZfQfaxw=='
& $AzCopy $SourceThumb $DestinationThumb /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationThumbKey /SetContentType:image/jpeg
