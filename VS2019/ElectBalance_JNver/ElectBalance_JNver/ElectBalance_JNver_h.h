

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Tue Jan 19 12:14:07 2038
 */
/* Compiler settings for ElectBalance_JNver.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.01.0622 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */



/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif /* __RPCNDR_H_VERSION__ */


#ifndef __ElectBalance_JNver_h_h__
#define __ElectBalance_JNver_h_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IElectBalance_JNver_FWD_DEFINED__
#define __IElectBalance_JNver_FWD_DEFINED__
typedef interface IElectBalance_JNver IElectBalance_JNver;

#endif 	/* __IElectBalance_JNver_FWD_DEFINED__ */


#ifndef __ElectBalance_JNver_FWD_DEFINED__
#define __ElectBalance_JNver_FWD_DEFINED__

#ifdef __cplusplus
typedef class ElectBalance_JNver ElectBalance_JNver;
#else
typedef struct ElectBalance_JNver ElectBalance_JNver;
#endif /* __cplusplus */

#endif 	/* __ElectBalance_JNver_FWD_DEFINED__ */


#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __ElectBalance_JNver_LIBRARY_DEFINED__
#define __ElectBalance_JNver_LIBRARY_DEFINED__

/* library ElectBalance_JNver */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_ElectBalance_JNver;

#ifndef __IElectBalance_JNver_DISPINTERFACE_DEFINED__
#define __IElectBalance_JNver_DISPINTERFACE_DEFINED__

/* dispinterface IElectBalance_JNver */
/* [uuid] */ 


EXTERN_C const IID DIID_IElectBalance_JNver;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("E778F383-4D99-4684-8FCC-DB5EECA0C146")
    IElectBalance_JNver : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct IElectBalance_JNverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IElectBalance_JNver * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IElectBalance_JNver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IElectBalance_JNver * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IElectBalance_JNver * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IElectBalance_JNver * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IElectBalance_JNver * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IElectBalance_JNver * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } IElectBalance_JNverVtbl;

    interface IElectBalance_JNver
    {
        CONST_VTBL struct IElectBalance_JNverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IElectBalance_JNver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IElectBalance_JNver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IElectBalance_JNver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IElectBalance_JNver_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IElectBalance_JNver_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IElectBalance_JNver_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IElectBalance_JNver_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* __IElectBalance_JNver_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_ElectBalance_JNver;

#ifdef __cplusplus

class DECLSPEC_UUID("0EAEC3DF-6495-4E6B-8C8E-B8997E31D9D4")
ElectBalance_JNver;
#endif
#endif /* __ElectBalance_JNver_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


