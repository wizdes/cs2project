using System;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CS2.Services.Updating
{
    public class UpdatingService : IUpdatingService
    {
        private readonly IndexReader indexReader;

        public IndexReader IndexReader
        {
            get { return indexReader; }
        }

        public bool NeedsUpdating
        {
            get
            {
                for(int i= 0; i < indexReader.NumDocs(); i++)
                {
                    Document document = indexReader.Document(i);

                    FileInfo file = new FileInfo(document.Get(FieldFactory.PathFieldName));
                    
                    if(!file.Exists)
                        return true;

                    DateTime phisicalModified = new DateTime(file.LastWriteTime.Year, file.LastWriteTime.Month, file.LastWriteTime.Day, file.LastWriteTime.Hour, file.LastWriteTime.Minute, file.LastWriteTime.Second);
                    DateTime dummyIndexed = DateTools.StringToDate(document.Get(FieldFactory.ModifiedFieldName));

                    DateTime indexedModified = new DateTime(dummyIndexed.Year, dummyIndexed.Month, dummyIndexed.Day, dummyIndexed.Hour, dummyIndexed.Minute, dummyIndexed.Second);

                    if(phisicalModified > indexedModified)
                        return true;
                }

                return false;
            }
        }

        public UpdatingService(IndexReader indexReader)
        {
            this.indexReader = indexReader;
        }

        #region IUpdatingService Members

        public void Update()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}