using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FourCDiffServer.Models;
using System;

namespace FourCDiffServer.Controllers {
    /// <summary>
    /// Handles HTTP requests for the diff functionality.
    /// </summary>
    [RoutePrefix("v1/diff")]
    public class DiffItemsController : ApiController {

        private IFourCDiffServerContext db = new FourCDiffServerContext();

        #region Constructors

        public DiffItemsController() { }

        public DiffItemsController(IFourCDiffServerContext context) {
            db = context;
        }

        #endregion

        #region Public methods

        // GET: v1/diff
        [Route("")]
        public IQueryable<DiffItem> GetDiffItems() {
            return db.DiffItems;
        }

        // GET: v1/diff/1
        [ResponseType(typeof(DiffResult))]
        [Route("{id}", Name = "GetDiffItemAsync")]
        public async Task<IHttpActionResult> GetDiffItemAsync(int id) {
            DiffItem diffItem = await db.DiffItems.FindAsync(id);
            if (diffItem == null || string.IsNullOrEmpty(diffItem.LeftSide) || string.IsNullOrEmpty(diffItem.RightSide)) {
                return NotFound();
            }

            string left = diffItem.LeftSide;
            string right = diffItem.RightSide;

            if (left == right) {
                return Ok(new DiffResult { DiffResultType = "Equals" });
            }
            if (left.Length != right.Length) {
                return Ok(new DiffResult { DiffResultType = "SizesDoNotMatch" });
            }
            DiffResult diffResult = new DiffResult { DiffResultType = "ContentsDoNotMatch" };
            diffResult.Diffs = DetermineDiffs(left, right);

            return Ok(diffResult);
        }

        // PUT: v1/diff/1/left
        [ResponseType(typeof(void))]
        [Route("{id}/left")]
        public async Task<IHttpActionResult> PutLeftSideAsync(int id, DiffInput diffInput) {
            return await SetLeftOrRightSideAsync(id, diffInput, true);
        }

        // PUT: v1/diff/1/right
        [ResponseType(typeof(void))]
        [Route("{id}/right")]
        public async Task<IHttpActionResult> PutRightSideAsync(int id, DiffInput diffInput) {
            return await SetLeftOrRightSideAsync(id, diffInput, false);
        }

        // DELETE: v1/diff/1
        [Route("{id}")]
        public HttpResponseMessage DeleteDiffItem(int id) {
            DiffItem diffItem = db.DiffItems.Find(id);
            if (diffItem == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.DiffItems.Remove(diffItem);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion

        #region Protected methods

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private methods

        private bool DiffItemExists(int id) {
            return db.DiffItems.Any(e => e.Id == id);
        }

        /// <summary>
        /// Sets the input for the left- or right side.
        /// </summary>
        private async Task<IHttpActionResult> SetLeftOrRightSideAsync(int id, DiffInput diffInput, bool isLeftSide) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (diffInput.Data == null) {
                return BadRequest();
            }

            DiffItem diffItem = await db.DiffItems.FindAsync(id);

            bool isAdded = false;
            bool isModified = false;
            if (diffItem == null) {
                diffItem = new DiffItem { Id = id };
                db.DiffItems.Add(diffItem);
                db.MarkAsAdded(diffItem);
                isAdded = true;
            } else {
                isModified = isLeftSide && diffItem.LeftSide != diffInput.Data || !isLeftSide && diffItem.RightSide != diffInput.Data;
                if (isModified) {
                    db.MarkAsModified(diffItem);
                }
            }

            if (isAdded || isModified) {
                if (isLeftSide) {
                    diffItem.LeftSide = diffInput.Data;
                } else {
                    diffItem.RightSide = diffInput.Data;
                }
                try {
                    await db.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException ex) {
                    if (!DiffItemExists(id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
            }
            return Created(new Uri("v1/diff/" + id, UriKind.RelativeOrAbsolute), diffInput);
        }

        /// <summary>
        /// Determines a list of differences (Offset-Length pairs) from two strings with the same length.
        /// </summary>
        /// <remarks>
        /// For example, comparing "AAAAAA==" to "ABCADA==" will result in { (1,2), (4,1) }.
        /// </remarks>
        private IEnumerable<Diff> DetermineDiffs(string left, string right) {
            List<Diff> diffs = new List<Diff>();
            bool processingDiff = false;
            int offset = 0;
            for (int i = 0; i < left.Length; i++) {
                if (left[i] != right[i]) {
                    if (!processingDiff) {
                        offset = i;
                        processingDiff = true;
                    }
                    if (i == left.Length - 1) {
                        diffs.Add(new Diff(offset, left.Length - offset));
                    }
                } else if (processingDiff) {
                    diffs.Add(new Diff(offset, i - offset));
                    processingDiff = false;
                }
            }
            return diffs;
        }

        #endregion
    }
}