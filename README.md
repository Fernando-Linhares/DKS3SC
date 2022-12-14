# DKS3SC

### Library to digital signature A1 and A3 pdf

## Overview

This lib provide support to signatures types A1 and A3 pdf.

## Dependencies

- ItextSharp
- Itext7

## Installation

To install you should compile this project using dotnet functionalities or visual studio
and import the assembly to the project.

> library name DKS3CS.dll to import

## Functionalities


### Namespaces
            using DKS3SC;
            using DKS3SC.Dependencies;
            using DKS3SC.Exceptions;

### Steps Development
The first step you need to intantiate the class main ``Signer``

            var signer = new Signer();

Next you must select certificate and use method

            signer.Select(id) # id of certificate

To see Certificate id an name

            signer.Certificates # return dictionary of id and name of certificates on slot

To select the files that must be signed use the method

            signer.FileInput # must be a string of file name
            /
            signer.FilesInput # must be a list of string of file names

The next step you should use the attribute to define the output files

            signer.FileOutput # must be a string of file name
            /
            signer.FilesOutput # must be a list of string of file names

Finally you should to use this methods to execute the signature

            signer.Sign() # execute signature throws exceptions of certificate validation
            /
            signer.SignAll() # execute signatures in all files throws exceptions of certificate validation

#### Configuring

Configuring signature. Is possible configure the apparence of signature to do this you
should do this steps below.

            Create class signature > Implements DKCS3SC.ISignatureDetails > Implements his methods

Next you should. in the attribute ``Visible``

            public bool Visible => true;

Set you configuration class on signer like described below

            signer.Apparence = new MySignatureApparence();

Now you can do your configuration on the method ``Apply`` as described

            public void Apply(Apparence apparence)
            {
                apparence.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
                apparence.SetVisibleSignature(new  iTextSharp.text.Rectangle(100, 100, 300, 200), 1, "Signature");
            }

You can configure image and more details follow the documentation of itext : https://itextpdf.com/
