/*M///////////////////////////////////////////////////////////////////////////////////////
//
//  IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING.
//
//  By downloading, copying, installing or using the software you agree to this license.
//  If you do not agree to this license, do not download, install,
//  copy or use the software.
//
//
//                        Intel License Agreement
//                For Open Source Computer Vision Library
//
// Copyright (C) 2000, Intel Corporation, all rights reserved.
// Third party copyrights are property of their respective owners.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   * Redistribution's of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   * Redistribution's in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//   * The name of Intel Corporation may not be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall the Intel Corporation or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//
//M*/
%include "exception.i"
%include "./pyhelpers.i"

%typemap(in) (CvArr *) (bool freearg=false){
	$1 = PyObject_to_CvArr($input, &freearg);
}
%typemap(freearg) (CvArr *) {
	if($1!=NULL && freearg$argnum){
		cvReleaseData( $1 );
		cvFree(&($1));
	}
}
%typecheck(SWIG_TYPECHECK_POINTER) CvArr * {
	void *ptr;
	if(PyList_Check($input) || PyTuple_Check($input)) {
		$1 = 1;
	}
	else if (SWIG_ConvertPtr($input, &ptr, 0, 0) == -1) {
		$1 = 0;
		PyErr_Clear();
	}
	else{
		$1 = 1;
	}
}

// for cvReshape, cvGetRow, where header is passed, then filled in
%typemap(in, numinputs=0) CvMat * OUTPUT (CvMat * header) {
	header = (CvMat *)cvAlloc(sizeof(CvMat));
   	$1 = header;
}
%newobject cvReshape;
%newobject cvGetRow;
%newobject cvGetRows;
%newobject cvGetCol;
%newobject cvGetCols;
%newobject cvGetSubRect;
%newobject cvGetDiag;

%apply CvMat *OUTPUT {CvMat * header};
%apply CvMat *OUTPUT {CvMat * submat};

/**
 * In C, these functions assume input will always be around at least as long as header,
 * presumably because the most common usage is to pass in a reference to a stack object.  
 * i.e
 * CvMat row;
 * cvGetRow(A, &row, 0);
 *
 * As a result, the header is not refcounted (see the C source for cvGetRow, Reshape, in cxarray.cpp)
 * However, in python, the header parameter is implicitly created so it is easier to create
 * situations where the sub-array outlives the original header.  A simple example is:
 * A = cvReshape(A, -1, A.rows*A.cols)
 *
 * since python doesn't have an assignment operator, the new header simply replaces the original,
 * the refcount of the original goes to zero, and cvReleaseMat is called on the original, freeing both
 * the header and data.  The new header is left pointing to invalid data.  To avoid this, need to add
 * refcount field to the returned header.
*/
%typemap(argout) (const CvArr* arr, CvMat* header) {
	$2->hdr_refcount = ((CvMat *)$1)->hdr_refcount;
	$2->refcount = ((CvMat *)$1)->refcount;
	cvIncRefData($2);
}
%typemap(argout) (const CvArr* arr, CvMat* submat) {
	$2->hdr_refcount = ((CvMat *)$1)->hdr_refcount;
	$2->refcount = ((CvMat *)$1)->refcount;
	cvIncRefData($2);
}

/* map scalar or sequence to CvScalar, CvPoint2D32f, CvPoint */
%typemap(in) (CvScalar) {
	$1 = PyObject_to_CvScalar( $input );
}
%typemap(in) (CvPoint) {
	$1 = PyObject_to_CvPoint($input);
}
%typemap(in) (CvPoint2D32f) {
	$1 = PyObject_to_CvPoint2D32f($input);
}


/* typemap for cvGetDims */
%typemap(in) (const CvArr * arr, int * sizes = NULL) (void * myarr, int mysizes[CV_MAX_DIM]){
	SWIG_Python_ConvertPtr($input, &myarr, 0, SWIG_POINTER_EXCEPTION);
	$1=(CvArr *)myarr;
	$2=mysizes;
}

%typemap(argout) (const CvArr * arr, int * sizes = NULL) {
	int len = PyInt_AsLong( $result );
	PyObject * obj = PyTuple_FromIntArray( $2, len );
	Py_DECREF( $result );
	$result = obj;
}
				
/* map one list of integer to the two parameters dimension/sizes */
%typemap(in) (int dims, int* sizes) {
    int i;

    /* get the size of the dimention array */
    $1 = PyList_Size ($input);

    /* allocate the needed memory */
    $2 = (int *)malloc ($1 * sizeof (int));

    /* extract all the integer values from the list */
    for (i = 0; i < $1; i++) {
	PyObject *item = PyList_GetItem ($input, i);
	$2 [i] = (int)PyInt_AsLong (item);
    }
}

/* map one list of integer to the parameter idx of
   cvGetND, cvSetND, cvClearND, cvGetRealND, cvSetRealND and cvClearRealND */
%typemap(in) (int* idx) {
    int i;
    int size;

    /* get the size of the dimention array */
    size = PyList_Size ($input);

    /* allocate the needed memory */
    $1 = (int *)malloc (size * sizeof (int));

    /* extract all the integer values from the list */
    for (i = 0; i < size; i++) {
	PyObject *item = PyList_GetItem ($input, i);
	$1 [i] = (int)PyInt_AsLong (item);
    }
}

/* map a list of list of float to an matrix of floats*/
%typemap(in) float** ranges {
    int i1;
    int i2;
    int size1;
    int size2 = 0;

    /* get the number of lines of the matrix */
    size1 = PyList_Size ($input);

    /* allocate the correct number of lines for the destination matrix */
    $1 = (float **)malloc (size1 * sizeof (float *));

    for (i1 = 0; i1 < size1; i1++) {

	/* extract all the lines of the matrix */
	PyObject *list = PyList_GetItem ($input, i1);

	if (size2 == 0) {
	    /* size 2 wasn't computed before */
	    size2 = PyList_Size (list);
	} else if (size2 != PyList_Size (list)) {
	    /* the current line as a different size than the previous one */
	    /* so, generate an exception */
	    SWIG_exception (SWIG_ValueError, "Lines must be the same size");
	}

	/* allocate the correct number of rows for the current line */
	$1 [i1] = (float *)malloc (size2 * sizeof (float));

	/* extract all the float values of this row */
	for (i2 = 0; i2 < size2; i2++) {
	    PyObject *item = PyList_GetItem (list, i2);
	    $1 [i1][i2] = (float)PyFloat_AsDouble (item);
	}
    }
}

/**
 * map the output parameter of the cvGetMinMaxHistValue()
 * so, we can call cvGetMinMaxHistValue() in Python like:
 * min_value, max_value = cvGetMinMaxHistValue (hist, None, None)
 */
%apply float *OUTPUT {float *min_value};
%apply float *OUTPUT {float *max_value};
/**
 * map output parameters of cvMinMaxLoc
 */
%apply double *OUTPUT {double* min_val, double* max_val};

/**
 * the input argument of cvPolyLine "CvPoint** pts" is converted from 
 * a "list of list" (aka. an array) of CvPoint().
 * The next parameters "int* npts" and "int contours" are computed from
 * the givne list.
 */
%typemap(in) (CvPoint** pts, int* npts, int contours){
    int i;
    int j;
    int size2 = -1;
    CvPoint **points = NULL;
    int *nb_vertex = NULL;

    /* get the number of polylines input array */
    int size1 = PyList_Size ($input);
    $3 = size1;

    for (i = 0; i < size1; i++) {

	/* get the current item */
	PyObject *line = PyList_GetItem ($input, i);

	/* get the size of the current polyline */
	size2 = PyList_Size (line);

	if (points == NULL) {
	    /* create the points array */
	    points = (CvPoint **)malloc (size1 * sizeof (CvPoint *));

	    /* point to the created array for passing info to the C function */
	    $1 = points;

	    /* create the array for memorizing the vertex */
	    nb_vertex = (int *)malloc (size1 * sizeof (int));
	    $2 = nb_vertex;

	}

	/* allocate the necessary memory to store the points */
	points [i] = (CvPoint *)malloc (size2 * sizeof (CvPoint));
	    
	/* memorize the size of the polyline in the vertex list */
	nb_vertex [i] = size2;

	for (j = 0; j < size2; j++) {
	    /* get the current item */
	    PyObject *item = PyList_GetItem (line, j);

	    /* convert from a Python CvPoint pointer to a C CvPoint pointer */
		void *vptr;
	    SWIG_Python_ConvertPtr (item, &vptr, $descriptor(CvPoint *),
				    SWIG_POINTER_EXCEPTION);
		
	    CvPoint *p = (CvPoint *) vptr;

	    /* extract the x and y positions */
	    points [i][j].x = p->x;
	    points [i][j].y = p->y;
	}
    }
}
/** Free arguments allocated before the function call */
%typemap(freearg) (CvPoint **pts, int* npts, int contours){
	int i;
	for(i=0;i<$3;i++){
		free($1[i]);
	}
	free($1);
	free($2);
}

/** 
 * The input argument of cvFillConvexPoly is convert from a list of CvPoints to a CvPoint array
 */
%typemap(in, numinputs=1) (CvPoint *pts, int npts){
	int i;
	int size = PyList_Size($input);
	CvPoint * points = (CvPoint *)malloc(size*sizeof(CvPoint));
	for(i=0; i<size; i++){
		PyObject *item = PyList_GetItem($input, i);
		points[i] = PyObject_to_CvPoint( item );
	}
	$1 = points;
	$2 = size;
}
/** Free arguments allocated before the function call */
%typemap(freearg) (CvPoint *pts, int npts){
	free((char *)$1);
}

/**
 * this is mainly an "output parameter"
 * So, just allocate the memory as input
 */
%typemap (in, numinputs=0) (CvSeq ** OUTPUT) (CvSeq * seq) {
    $1 = &seq;
}

/**
 * return the finded contours with all the others parametres
 */
%typemap(argout) (CvSeq ** OUTPUT) {
    PyObject *to_add;

    /* extract the pointer we want to add to the returned tuple */
	/* sequence is allocated in CvMemStorage, so python_ownership=0 */
    to_add = SWIG_NewPointerObj (*$1, $descriptor(CvSeq*), 0); 

	$result = SWIG_AppendResult($result, &to_add, 1);
}
%apply CvSeq **OUTPUT {CvSeq **first_contour};
%apply CvSeq **OUTPUT {CvSeq **comp};

/**
 * CvArr ** image can be either one CvArr or one array of CvArr
 * (for example like in cvCalcHist() )
 * From Python, the array of CvArr can be a tuple.
 */
%typemap(in) (CvArr ** INPUT) (
	CvArr * one_image=NULL, 
	bool free_one_arg=false, 
	CvArr ** many_images=NULL, 
	bool *free_many_args=NULL, 
	int nimages=0 ) {

    /* first, check if this is just one CvArr */
    /* if this is just one CvArr * one_image will receive it */
	if( (one_image = PyObject_to_CvArr( $input, &free_one_arg )) ){
		$1 = &one_image;
	}
    else if PyTuple_Check ($input) {
		/* This is a tuple, so we need to test each element and pass
	   		them to the called function */

	int i;

	/* get the size of the tuple */
	nimages = PyTuple_Size ($input);

	/* allocate the necessary place */
	many_images = (CvArr **)malloc (nimages * sizeof (CvArr *));
	free_many_args = (bool *)malloc(nimages * sizeof(bool));

	for (i = 0; i < nimages; i++) {

	    /* convert the current tuple element to a CvArr *, and
	       store to many_images [i] */
		many_images[i] = PyObject_to_CvArr(PyTuple_GetItem ($input, i), free_many_args+i);

	    /* check that the current item is a correct type */
	    if(!many_images[i]) {
			/* incorrect ! */
			SWIG_fail;
	    }
	}

	/* what to give to the called function */
	$1 = many_images;

    } else {
		/* not a CvArr *, not a tuple, this is wrong */
		SWIG_fail;
    }
}
%apply CvArr ** INPUT {CvArr ** image};
%apply CvArr ** INPUT {CvArr ** arr};
%apply CvArr ** INPUT {CvArr ** vects};

%typemap(freearg) (CvArr ** FREEARG) {
	if(free_one_arg$argnum){
		cvFree(&(one_image$argnum));
	}
	else if(free_many_args$argnum){
		int i;
		for (i=0; i<nimages$argnum; i++){
			if(free_many_args$argnum[i]){
				cvReleaseData(many_images$argnum[i]);
				cvFree(many_images$argnum+i);
			}
		}
		free(many_images$argnum);
		free(free_many_args$argnum);
	}

}
%apply CvArr ** FREEARG {CvArr ** image};
%apply CvArr ** FREEARG {CvArr ** arr};
%apply CvArr ** FREEARG {CvArr ** vects};

/**
 * Map the CvFont * parameter from the cvInitFont() as an output parameter
 */
%typemap (in, numinputs=1) (CvFont* font, int font_face) {
    $1 = (CvFont *)malloc (sizeof (CvFont));
    $2 = (int)PyInt_AsLong ($input); 
    if (SWIG_arg_fail($argnum)) SWIG_fail;
}
%typemap(argout) (CvFont* font, int font_face) {
    PyObject *to_add;

    /* extract the pointer we want to add to the returned tuple */
    to_add = SWIG_NewPointerObj ($1, $descriptor(CvFont *), 0);

	$result = SWIG_AppendResult($result, &to_add, 1);
}

/**
 * these are output parameters for cvGetTextSize
 */
%typemap (in, numinputs=0) (CvSize* text_size, int* baseline) {
    CvSize *size = (CvSize *)malloc (sizeof (CvSize));
    int *baseline = (int *)malloc (sizeof (int));
    $1 = size;
    $2 = baseline;
}

/**
 * return the finded parameters for cvGetTextSize
 */
%typemap(argout) (CvSize* text_size, int* baseline) {
    PyObject * to_add[2];

    /* extract the pointers we want to add to the returned tuple */
    to_add [0] = SWIG_NewPointerObj ($1, $descriptor(CvSize *), 0);
    to_add [1] = PyInt_FromLong (*$2);

    $result = SWIG_AppendResult($result, to_add, 2);
}


/**
 * curr_features is output parameter for cvCalcOpticalFlowPyrLK
 */
%typemap (in, numinputs=1) (CvPoint2D32f* curr_features, int count)
     (int tmpCount) {
    /* as input, we only need the size of the wanted features */

    /* memorize the size of the wanted features */
    tmpCount = (int)PyInt_AsLong ($input);

    /* create the array for the C call */
    $1 = (CvPoint2D32f *) malloc(tmpCount * sizeof (CvPoint2D32f));

    /* the size of the array for the C call */
    $2 = tmpCount;
}

/**
 * the features returned by cvCalcOpticalFlowPyrLK
 */
%typemap(argout) (CvPoint2D32f* curr_features, int count) {
    int i;
    PyObject *to_add;
    
    /* create the list to return */
    to_add = PyList_New (tmpCount$argnum);

    /* extract all the points values of the result, and add it to the
       final resulting list */
    for (i = 0; i < tmpCount$argnum; i++) {
	PyList_SetItem (to_add, i,
			SWIG_NewPointerObj (&($1 [i]),
					    $descriptor(CvPoint2D32f *), 0));
    }

	$result = SWIG_AppendResult($result, &to_add, 1);
}

/**
 * status is an output parameters for cvCalcOpticalFlowPyrLK
 */
%typemap (in, numinputs=1) (char *status) (int tmpCountStatus){
    /* as input, we still need the size of the status array */

    /* memorize the size of the status array */
    tmpCountStatus = (int)PyInt_AsLong ($input);

    /* create the status array for the C call */
    $1 = (char *)malloc (tmpCountStatus * sizeof (char));
}

/**
 * the status returned by cvCalcOpticalFlowPyrLK
 */
%typemap(argout) (char *status) {
    int i;
    PyObject *to_add;

    /* create the list to return */
    to_add = PyList_New (tmpCountStatus$argnum);

    /* extract all the integer values of the result, and add it to the
       final resulting list */
    for (i = 0; i < tmpCountStatus$argnum; i++) {
		PyList_SetItem (to_add, i, PyBool_FromLong ($1 [i]));
    }

	$result = SWIG_AppendResult($result, &to_add, 1); 
}

/* map one list of points to the two parameters dimenssion/sizes
 for cvCalcOpticalFlowPyrLK */
%typemap(in) (CvPoint2D32f* prev_features) {
    int i;
    int size;

    /* get the size of the input array */
    size = PyList_Size ($input);

    /* allocate the needed memory */
    $1 = (CvPoint2D32f *)malloc (size * sizeof (CvPoint2D32f));

    /* extract all the points values from the list */
    for (i = 0; i < size; i++) {
	PyObject *item = PyList_GetItem ($input, i);

	void * vptr;
	SWIG_Python_ConvertPtr (item, &vptr,
				$descriptor(CvPoint2D32f*),
				SWIG_POINTER_EXCEPTION);
	CvPoint2D32f *p = (CvPoint2D32f *)vptr;
	$1 [i].x = p->x;
	$1 [i].y = p->y;
    }
}

/**
 * the corners returned by cvGoodFeaturesToTrack
 */
%typemap (in, numinputs=1) (CvPoint2D32f* corners, int* corner_count)
     (int tmpCount) {
    /* as input, we still need the size of the corners array */

    /* memorize the size of the status corners */
    tmpCount = (int)PyInt_AsLong ($input);

    /* create the corners array for the C call */
    $1 = (CvPoint2D32f *)malloc (tmpCount * sizeof (CvPoint2D32f));

    /* the size of the array for the C call */
    $2 = &tmpCount;
}

/**
 * the corners returned by cvGoodFeaturesToTrack
 */
%typemap(argout) (CvPoint2D32f* corners, int* corner_count) {
    int i;
    PyObject *to_add;
    
    /* create the list to return */
    to_add = PyList_New (tmpCount$argnum);

    /* extract all the integer values of the result, and add it to the
       final resulting list */
    for (i = 0; i < tmpCount$argnum; i++) {
	PyList_SetItem (to_add, i,
			SWIG_NewPointerObj (&($1 [i]),
					    $descriptor(CvPoint2D32f *), 0));
    }

    $result = SWIG_AppendResult($result, &to_add, 1);
}

/* map one list of points to the two parameters dimension/sizes
   for cvFindCornerSubPix */
%typemap(in, numinputs=1) (CvPoint2D32f* corners, int count)
     (int cornersCount, CvPoint2D32f* corners){
    int i;

	if(!PyList_Check($input)){
		PyErr_SetString(PyExc_TypeError, "Expected a list");
		return NULL;
	}

    /* get the size of the input array */
    cornersCount = PyList_Size ($input);
    $2 = cornersCount;

    /* allocate the needed memory */
    corners = (CvPoint2D32f *)malloc ($2 * sizeof (CvPoint2D32f));
    $1 = corners;

    /* the size of the array for the C call */

    /* extract all the points values from the list */
    for (i = 0; i < $2; i++) {
	PyObject *item = PyList_GetItem ($input, i);

	void *vptr;
	SWIG_Python_ConvertPtr (item, &vptr,
				$descriptor(CvPoint2D32f*),
				SWIG_POINTER_EXCEPTION);
	CvPoint2D32f *p = (CvPoint2D32f *) vptr;;
	$1 [i].x = p->x;
	$1 [i].y = p->y;
    }

}

/**
 * the corners returned by cvFindCornerSubPix
 */
%typemap(argout) (CvPoint2D32f* corners, int count) {
    int i;
    PyObject *to_add;

    /* create the list to return */
    to_add = PyList_New (cornersCount$argnum);

    /* extract all the corner values of the result, and add it to the
       final resulting list */
    for (i = 0; i < cornersCount$argnum; i++) {
	PyList_SetItem (to_add, i,
			SWIG_NewPointerObj (&(corners$argnum [i]),
					    $descriptor(CvPoint2D32f *), 0));
    }

	$result = SWIG_AppendResult( $result, &to_add, 1);
}

/**
 * return the corners for cvFindChessboardCorners
 */
%typemap(in, numinputs=1) (CvSize pattern_size, CvPoint2D32f * corners, int * corner_count ) 
     (CvSize * pattern_size, CvPoint2D32f * tmp_corners, int tmp_ncorners) {
	 void * vptr;
	if( SWIG_ConvertPtr($input, &vptr, $descriptor( CvSize * ), SWIG_POINTER_EXCEPTION ) == -1){
		return NULL;
	}
	pattern_size=(CvSize *)vptr;
	tmp_ncorners = pattern_size->width*pattern_size->height;

	tmp_corners = (CvPoint2D32f *) malloc(sizeof(CvPoint2D32f)*tmp_ncorners);
	$1 = *pattern_size;
	$2 = tmp_corners;
	$3 = &tmp_ncorners;
}

%typemap(argout) (CvSize pattern_size, CvPoint2D32f * corners, int * corner_count){
    int i;
    PyObject *to_add;

    /* create the list to return */
    to_add = PyList_New ( tmp_ncorners$argnum );

    /* extract all the corner values of the result, and add it to the
       final resulting list */
    for (i = 0; i < tmp_ncorners$argnum; i++) {
		CvPoint2D32f * pt = new CvPoint2D32f;
		pt->x = tmp_corners$argnum[i].x;
		pt->y = tmp_corners$argnum[i].y;
		
    	PyList_SetItem (to_add, i,
            SWIG_NewPointerObj( pt, $descriptor(CvPoint2D32f *), 0));
    }

	$result = SWIG_AppendResult( $result, &to_add, 1);
    free(tmp_corners$argnum);
}

/**
 * return the matrices for cvCameraCalibrate
 */
%typemap(in, numinputs=0) (CvMat * intrinsic_matrix, CvMat * distortion_coeffs)
{
	$1 = cvCreateMat(3,3,CV_32F);
	$2 = cvCreateMat(4,1,CV_32F);
}

%typemap(argout) (CvMat * intrinsic_matrix, CvMat * distortion_coeffs)
{
	PyObject * to_add[2] = {NULL, NULL};
	to_add[0] = SWIG_NewPointerObj($1, $descriptor(CvMat *), 1);
	to_add[1] = SWIG_NewPointerObj($2, $descriptor(CvMat *), 1);
	$result = SWIG_AppendResult( $result, to_add, 2 );
}

/**
 * Fix OpenCV inheritance for CvSeq, CvSet, CvGraph
 * Otherwise, can't call CvSeq functions on CvSet or CvGraph
*/
%typemap(in, numinputs=1) (CvSeq *) (void * ptr)
{
	
	if( SWIG_ConvertPtr($input, &ptr, $descriptor(CvSeq *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvSet *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvGraph *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvSubdiv2D *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvChain *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvContour *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvContourTree *), 0) == -1 )
	{
		SWIG_exception (SWIG_TypeError, "could not convert to CvSeq");
		return NULL;
	}
	$1 = (CvSeq *) ptr;
}

%typemap(in, numinputs=1) (CvSet *) (void * ptr)
{
	if( SWIG_ConvertPtr($input, &ptr, $descriptor(CvSet *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvGraph *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvSubdiv2D *), 0) == -1) 
	{
		SWIG_exception (SWIG_TypeError, "could not convert to CvSet");
		return NULL;
	}
	$1 = (CvSet *)ptr;
}

%typemap(in, numinputs=1) (CvGraph *) (void * ptr)
{
	if( SWIG_ConvertPtr($input, &ptr, $descriptor(CvGraph *), 0) == -1 &&
	    SWIG_ConvertPtr($input, &ptr, $descriptor(CvSubdiv2D *), 0) == -1) 
	{
		SWIG_exception (SWIG_TypeError, "could not convert to CvGraph");
		return NULL;
	}
	$1 = (CvGraph *)ptr;
}

/**
 * Remap output arguments to multiple return values for cvMinEnclosingCircle
 */
%typemap(in, numinputs=0) (CvPoint2D32f * center, float * radius) (CvPoint2D32f * tmp_center, float tmp_radius) 
{
	tmp_center = (CvPoint2D32f *) malloc(sizeof(CvPoint2D32f));
	$1 = tmp_center;
	$2 = &tmp_radius;
}
%typemap(argout) (CvPoint2D32f * center, float * radius)
{
    PyObject * to_add[2] = {NULL, NULL};
	to_add[0] = SWIG_NewPointerObj( tmp_center$argnum, $descriptor(CvPoint2D32f *), 1); 
	to_add[1] = PyFloat_FromDouble( tmp_radius$argnum );

    $result = SWIG_AppendResult($result, to_add, 2);
}

/** BoxPoints */
%typemap(in, numinputs=0) (CvPoint2D32f pt[4]) (CvPoint2D32f tmp_pts[4])
{
	$1 = tmp_pts;
}
%typemap(argout) (CvPoint2D32f pt[4])
{
	PyObject * to_add = PyList_New(4);
	int i;
	for(i=0; i<4; i++){
		CvPoint2D32f * p = new CvPoint2D32f;
		*p = tmp_pts$argnum[i];
		PyList_SetItem(to_add, i, SWIG_NewPointerObj( p, $descriptor(CvPoint2D32f *), 1 ) );
	}
	$result = SWIG_AppendResult($result, &to_add, 1);
}

/** Macro to wrap a built-in type that is used as an object like CvRNG and CvSubdiv2DEdge */
%define %wrap_builtin(type)
%inline %{
// Wrapper class
class type##_Wrapper {
private:
	type m_val;
public:
	type##_Wrapper( const type & val ) :
		m_val(val)
	{
	}
	type * ptr() { return &m_val; }
	type & ref() { return m_val; }
	bool operator==(const type##_Wrapper & x){
		return m_val==x.m_val;
	}
	bool operator!=(const type##_Wrapper & x){
		return m_val!=x.m_val;
	}
};
%}
%typemap(out) type
{
	type##_Wrapper * wrapper = new type##_Wrapper( $1 );
	$result = SWIG_NewPointerObj( wrapper, $descriptor( type##_Wrapper * ), 1 );
}
%typemap(out) type *
{
	type##_Wrapper * wrapper = new type##_Wrapper( *($1) );
	$result = SWIG_NewPointerObj( wrapper, $descriptor( type##_Wrapper * ), 1 );
}

%typemap(in) (type *) (void * vptr, type##_Wrapper * wrapper){
	if(SWIG_ConvertPtr($input, &vptr, $descriptor(type##_Wrapper *), 0)==-1){
		SWIG_exception( SWIG_TypeError, "could not convert Python object to C value");
		return NULL;
	}
	wrapper = (type##_Wrapper *) vptr;
	$1 = wrapper->ptr();
}
%typemap(in) (type) (void * vptr, type##_Wrapper * wrapper){
	if(SWIG_ConvertPtr($input, &vptr, $descriptor(type##_Wrapper *), 0)==-1){
		SWIG_exception( SWIG_TypeError, "could not convert Python object to C value");
		return NULL;
	}
	wrapper = (type##_Wrapper *) vptr;
	$1 = wrapper->ref();
}
%enddef 

/** Application of wrapper class to built-in types */
%wrap_builtin(CvRNG);
%wrap_builtin(CvSubdiv2DEdge);

/**
 * Allow CvQuadEdge2D to be interpreted as CvSubdiv2DEdge
 */
%typemap(in, numinputs=1) (CvSubdiv2DEdge) (CvSubdiv2DEdge_Wrapper * wrapper, CvQuadEdge2D * qedge, void *vptr)
{
	if( SWIG_ConvertPtr($input, &vptr, $descriptor(CvSubdiv2DEdge_Wrapper *), 0) != -1 ){
		wrapper = (CvSubdiv2DEdge_Wrapper *) vptr;
		$1 = wrapper->ref();
	}
	else if( SWIG_ConvertPtr($input, &vptr, $descriptor(CvQuadEdge2D *), 0) != -1 ){
		qedge = (CvQuadEdge2D *) vptr;
		$1 = (CvSubdiv2DEdge)qedge;
	}
	else{
		 SWIG_exception( SWIG_TypeError, "could not convert to CvSubdiv2DEdge");
		 return NULL;
	}
}

/**
 * return the vertex and edge for cvSubdiv2DLocate
 */
%typemap(in, numinputs=0) (CvSubdiv2DEdge * edge, CvSubdiv2DPoint ** vertex) 
	(CvSubdiv2DEdge tmpEdge, CvSubdiv2DPoint * tmpVertex)
{
	$1 = &tmpEdge;
	$2 = &tmpVertex;
}
%typemap(argout) (CvSubdiv2DEdge * edge, CvSubdiv2DPoint ** vertex)
{
	PyObject * to_add[2] = {NULL, NULL};
	if(result==CV_PTLOC_INSIDE || result==CV_PTLOC_ON_EDGE){
		CvSubdiv2DEdge_Wrapper * wrapper = new CvSubdiv2DEdge_Wrapper( tmpEdge$argnum );
		to_add[0] = SWIG_NewPointerObj( wrapper, $descriptor(CvSubdiv2DEdge_Wrapper *), 0);
		to_add[1] = Py_None;
	}
	if(result==CV_PTLOC_VERTEX){
		to_add[0] = Py_None;
		to_add[1] = SWIG_NewPointerObj( tmpVertex$argnum, $descriptor(CvSubdiv2DPoint *), 0);
	}
	
	$result = SWIG_AppendResult($result, to_add, 2);
}

