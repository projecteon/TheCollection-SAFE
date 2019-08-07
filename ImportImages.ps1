$Source='/Source:C:\src\Theedatabase\Afbeeldingen Zakjes'
$MissingSource='/Source:C:\src\projecteon\missingthees'
$Destination='/Dest:http://127.0.0.1:10000/devstoreaccount1/images'
$DestinationKey='Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=='

$AzCopy='C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe'

& $AzCopy $Source $Destination /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationKey /SetContentType:image/jpeg
